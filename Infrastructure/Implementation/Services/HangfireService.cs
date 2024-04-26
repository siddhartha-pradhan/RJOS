using System.Globalization;
using Hangfire;
using System.Text;
using Newtonsoft.Json;
using Common.Utilities;
using Application.DTOs.Authentication;
using Application.DTOs.Student;
using Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Data.Implementation.Services;

public class HangfireService : IHostedService
{
    private readonly RsosSettings _rsosSettings;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly HangfireSettings _hangfireSettings;

    public HangfireService(IOptions<RsosSettings> rsosSettings, IOptions<HangfireSettings> hangfireSettings, IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _rsosSettings = rsosSettings.Value;
        _hangfireSettings = hangfireSettings.Value;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var jobOptions = new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Local,
            MisfireHandling = MisfireHandlingMode.Strict
        };
        
        RecurringJob.AddOrUpdate("ePCP", () => InsertData(), Cron.Daily(15, 3), jobOptions);
        
        return Task.CompletedTask;
    }

    public async Task InsertData()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
        
            var genericRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository>();
            
            var pcpStudentScores = 
                await genericRepository.GetAsync<tblStudentScore>(x => 
                    x.TopicId == 0 && !x.IsUploaded);

            var studentScores = pcpStudentScores as tblStudentScore[] ?? pcpStudentScores.ToArray();

            var students = 
                await genericRepository.GetAsync<tblStudentLoginHistory>(x => 
                    studentScores.Select(z => z.StudentId).Contains(x.StudentId ?? 0));

            foreach (var student in students)
            {
                if (student.SSOID == null || student.DateOfBirth == null || student.StudentId == null) continue;
                
                var httpClient = new HttpClient();

                var rsosToken = _rsosSettings.Token;

                var rsosUrl = _rsosSettings.URL;

                var loginBaseUrl = $"{rsosUrl}/new_api_student_login";

                var loginQueryParams = new System.Collections.Specialized.NameValueCollection
                {
                    { "ssoid", student.SSOID },
                    { "dob", student.DateOfBirth },
                    { "token", rsosToken }
                };

                var loginUriBuilder = new UriBuilder(loginBaseUrl)
                {
                    Query = string.Join("&", Array.ConvertAll(loginQueryParams.AllKeys,
                        key => $"{Uri.EscapeDataString(key!)}={Uri.EscapeDataString(loginQueryParams[key]!)}"))
                };

                var loginPostData = new StringContent("{\"key\": \"value\"}", Encoding.UTF8, "application/json");

                var loginResponse = await httpClient.PostAsync(loginUriBuilder.Uri, loginPostData);

                if (loginResponse.IsSuccessStatusCode)
                {
                    var loginResponseData = await loginResponse.Content.ReadAsStringAsync();

                    var loginApiResponse = JsonConvert.DeserializeObject<RSOSLoginResponse>(loginResponseData);

                    if (loginApiResponse is { Status: true })
                    {
                        var studentResponseData = JsonConvert.DeserializeObject<LoginResponseDTO>(loginResponseData);

                        var studentLoginData = studentResponseData!.Data.Student;

                        var secureToken = studentResponseData.secure_token;
                        
                        var pcpBaseUrl = $"{rsosUrl}/new_api_set_student_sessional_exam_subject_marks";

                        foreach (var score in studentScores)
                        {
                            if(DateTime.Now.Hour == 6) return;
                            
                            if (decimal.TryParse(score.Score, out var result))
                            {
                                var studentScore = result / 10;
                                
                                var pcpQueryParams = new System.Collections.Specialized.NameValueCollection
                                {
                                    { "token", rsosToken },
                                    { "secure_token", secureToken},
                                    { "enrollment", studentLoginData.Enrollment },
                                    { "ssoid", student.SSOID },
                                    { "subject_id", score.SubjectId.ToString() },
                                    { "obtained_marks", studentScore.ToString(CultureInfo.InvariantCulture) }
                                };
                            
                                var pcpUriBuilder = new UriBuilder(pcpBaseUrl)
                                {
                                    Query = string.Join("&", Array.ConvertAll(pcpQueryParams.AllKeys,
                                        key => $"{Uri.EscapeDataString(key!)}={Uri.EscapeDataString(pcpQueryParams[key]!)}"))
                                };

                                var pcpPostData = new StringContent("{\"key\": \"value\"}", Encoding.UTF8, "application/json");

                                var pcpResponse = await httpClient.PostAsync(pcpUriBuilder.Uri, pcpPostData);

                                if (pcpResponse.IsSuccessStatusCode)
                                {
                                    var pcpResponseData = await pcpResponse.Content.ReadAsStringAsync();

                                    var pcpApiResponse = JsonConvert.DeserializeObject<StudentSessionalMarksDTO>(pcpResponseData);

                                    if (pcpApiResponse is { Status: true })
                                    {
                                        score.IsUploaded = true;
                                        score.LastUpdatedOn = DateTime.Now;
                                        
                                        await genericRepository.UpdateAsync(score);   
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            await InsertData();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}