using System.Globalization;
using Application.DTOs.Authentication;
using Application.DTOs.Student;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Common.Utilities;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Data.Implementation.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly JwtSettings _jwtSettings;
    private readonly RsosSettings _rsosSettings;
    private readonly IGenericRepository _genericRepository;
    private readonly AuthenticationSettings _authenticationSettings;

    public AuthenticationService(IGenericRepository genericRepository, 
        IOptions<JwtSettings> jwtSettings, 
        IOptions<RsosSettings> rsosSettings, 
        IOptions<AuthenticationSettings> authenticationSettings)
    {
        _genericRepository = genericRepository;
        _jwtSettings = jwtSettings.Value;
        _rsosSettings = rsosSettings.Value;
        _authenticationSettings = authenticationSettings.Value;
    }

    public async Task<AuthenticationResponseDTO> Authenticate(AuthenticationRequestDTO authenticationRequest)
    {
        var httpClient = new HttpClient();

        var rsosToken = _rsosSettings.Token;

        var rsosUrl = _rsosSettings.URL;

        var baseUrl = $"{rsosUrl}/new_api_student_login";

        var queryParams = new System.Collections.Specialized.NameValueCollection
        {
            { "ssoid", authenticationRequest.SSOID },
            { "dob", authenticationRequest.DateOfBirth },
            { "token", rsosToken }
        };

        var uriBuilder = new UriBuilder(baseUrl)
        {
            Query = string.Join("&", Array.ConvertAll(queryParams.AllKeys,
                key => $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(queryParams[key])}"))
        };

        var postData = new StringContent("{\"key\": \"value\"}", Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(uriBuilder.Uri, postData);

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonConvert.DeserializeObject<RSOSLoginResponse>(responseData);
            
            if (apiResponse is { Status: true })
            {
                var studentResponseData = JsonConvert.DeserializeObject<LoginResponseDTO>(responseData);

                var studentLoginData = studentResponseData!.Data.Student;

                var existingStudentLoginHistory =
                    await _genericRepository.GetFirstOrDefaultAsync<tblStudentLoginHistory>(x =>
                        x.SSOID == studentLoginData.SsoId);

                if (existingStudentLoginHistory == null)
                {
                    var studentLoginHistoryModel = new tblStudentLoginHistory()
                    {
                        SSOID = studentLoginData.SsoId,
                        AttemptCount = 0,
                        LastAccessedTime = DateTime.Now,
                        StudentId = studentLoginData.Id,
                        Enrollment = studentLoginData.Enrollment,
                        DateOfBirth = studentLoginData.Dob.ToString("yyyy-MM-dd")
                    };

                    await InsertStudentLoginDetails(studentLoginData.SsoId, authenticationRequest.DeviceRegistrationToken);
                    
                    await _genericRepository.InsertAsync(studentLoginHistoryModel);
                }
                
                var studentLoginHistory =
                    await _genericRepository.GetFirstOrDefaultAsync<tblStudentLoginHistory>(x =>
                        x.SSOID == studentLoginData.SsoId);

                if (studentLoginHistory is { AttemptCount: >= 5 })
                {
                    if (studentLoginHistory.LastAccessedTime.AddMinutes(_authenticationSettings.DurationInMinutes) <= DateTime.Now)
                    {
                        var pcpDates =  await _genericRepository.GetAsync<tblPCPDate>(x => x.IsActive);

                        var maxPcpDate = pcpDates.MaxBy(x => x.Id);

                        var authenticationResponse = new AuthenticationResponseDTO
                        {
                            Id = studentLoginData.Id,
                            Enrollment = studentLoginData.Enrollment,
                            Name = studentLoginData.Name,
                            DateOfBirth = studentLoginData.Dob.ToString("yyyy-MM-dd"),
                            SSOID = studentLoginData.SsoId,
                            ApplicationToken = GenerateJwtToken(studentLoginData),
                            SecureRSOSToken = studentResponseData.secure_token,
                            ValidTill = studentResponseData.secure_token_valid_till,
                            StartDate = maxPcpDate != null ? maxPcpDate.StartDate.ToString("yyyy-MM-dd h:mm:ss tt") : "",
                            EndDate = maxPcpDate != null ? maxPcpDate!.EndDate.ToString("yyyy-MM-dd h:mm:ss tt") : ""
                        };

                        studentLoginHistory.AttemptCount = 0;
                        studentLoginHistory.StudentId = studentLoginData.Id;
                        studentLoginHistory.LastAccessedTime = DateTime.Now;
                        studentLoginHistory.Enrollment = studentLoginData.Enrollment;
                        studentLoginHistory.DateOfBirth = studentLoginData.Dob.ToString("yyyy-MM-dd");

                        await _genericRepository.UpdateAsync(studentLoginHistory);
                        
                        await InsertStudentLoginDetails(studentLoginData.SsoId, authenticationRequest.DeviceRegistrationToken);
                        
                        return authenticationResponse;
                    }

                    return new AuthenticationResponseDTO()
                    {
                        Id = -1,
                        ValidTill =  studentLoginHistory.LastAccessedTime.AddMinutes(_authenticationSettings.DurationInMinutes).ToString("dd-MM-yyyy hh:mm:ss tt")
                    };
                }
                else
                {
                    var pcpDates =  await _genericRepository.GetAsync<tblPCPDate>();

                    var maxPcpDate = pcpDates.MaxBy(x => x.Id);

                    var authenticationResponse = new AuthenticationResponseDTO
                    {
                        Id = studentLoginData.Id,
                        Enrollment = studentLoginData.Enrollment,
                        Name = studentLoginData.Name,
                        DateOfBirth = studentLoginData.Dob.ToString("yyyy-MM-dd"),
                        SSOID = studentLoginData.SsoId,
                        ApplicationToken = GenerateJwtToken(studentLoginData),
                        SecureRSOSToken = studentResponseData.secure_token,
                        ValidTill = studentResponseData.secure_token_valid_till,
                        StartDate = maxPcpDate != null ? maxPcpDate.StartDate.ToString("yyyy-MM-dd h:mm:ss tt") : "",
                        EndDate = maxPcpDate != null ? maxPcpDate!.EndDate.ToString("yyyy-MM-dd h:mm:ss tt") : ""
                    };

                    if (studentLoginHistory != null)
                    {
                        studentLoginHistory.AttemptCount = 0;
                        studentLoginHistory.LastAccessedTime = DateTime.Now;
                        studentLoginHistory.StudentId = studentLoginData.Id;
                        studentLoginHistory.Enrollment = studentLoginData.Enrollment;
                        studentLoginHistory.DateOfBirth = studentLoginData.Dob.ToString("yyyy-MM-dd");

                        await _genericRepository.UpdateAsync(studentLoginHistory);
                    }

                    await InsertStudentLoginDetails(studentLoginData.SsoId, authenticationRequest.DeviceRegistrationToken);
                    
                    return authenticationResponse;
                }
            }
            else
            {
                var existingStudentLoginHistory =
                    await _genericRepository.GetFirstOrDefaultAsync<tblStudentLoginHistory>(x =>
                        x.SSOID == authenticationRequest.SSOID);

                if (existingStudentLoginHistory == null)
                {
                    var studentLoginHistoryModel = new tblStudentLoginHistory()
                    {
                        SSOID = authenticationRequest.SSOID,
                        AttemptCount = 1,
                        LastAccessedTime = DateTime.Now,
                        Enrollment = "",
                        DateOfBirth = ""
                    };

                    await _genericRepository.InsertAsync(studentLoginHistoryModel);
                }
                else
                {
                    var studentLoginHistory = await _genericRepository.GetFirstOrDefaultAsync<tblStudentLoginHistory>(x =>
                        x.SSOID == authenticationRequest.SSOID);

                    if (studentLoginHistory != null)
                    {
                        if (studentLoginHistory.AttemptCount == 5)
                        {
                            if (studentLoginHistory.LastAccessedTime.AddMinutes(_authenticationSettings.DurationInMinutes) <= DateTime.Now)
                            {
                                studentLoginHistory.AttemptCount = 1;
                                studentLoginHistory.LastAccessedTime = DateTime.Now;

                                await _genericRepository.UpdateAsync(studentLoginHistory);

                                return new AuthenticationResponseDTO();
                            }

                            return new AuthenticationResponseDTO()
                            {
                                Id = -1,
                                ValidTill =  studentLoginHistory.LastAccessedTime.AddMinutes(_authenticationSettings.DurationInMinutes).ToString("dd-MM-yyyy hh:mm:ss tt")
                            };
                        }

                        studentLoginHistory.AttemptCount++; 
                        studentLoginHistory.LastAccessedTime = DateTime.Now;

                        await _genericRepository.UpdateAsync(studentLoginHistory);
                        
                        if (studentLoginHistory.AttemptCount == 5)
                        {
                            return new AuthenticationResponseDTO()
                            {
                                Id = -1,
                                ValidTill =  studentLoginHistory.LastAccessedTime.AddMinutes(_authenticationSettings.DurationInMinutes).ToString("dd-MM-yyyy hh:mm:ss tt")
                            };
                        }
                    }
                }

                return new AuthenticationResponseDTO();
                
            }
        }

        return new AuthenticationResponseDTO();
    }

    private string GenerateJwtToken(StudentInfoDTO studentInfo)
    {
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

        var issuer = _jwtSettings.Issuer;

        var audience = _jwtSettings.Audience;

        var durationInDays = Convert.ToInt32(_jwtSettings.DurationInDays);

        var authClaims = new List<Claim>
        {
            new("studentid", studentInfo.Id.ToString()),
            new("enrollment", studentInfo.Enrollment),
            new("dob", studentInfo.Dob.ToString("yyyy-MM-dd")),
            new("ssoid", studentInfo.SsoId.ToUpper()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var symmetricSigningKey = new SymmetricSecurityKey(key);

        var signingCredentials = new SigningCredentials(symmetricSigningKey, SecurityAlgorithms.HmacSha256);

        var expirationTime = DateTime.UtcNow.AddDays(durationInDays);

        var accessToken = new JwtSecurityToken(
            issuer,
            audience,
            claims: authClaims,
            signingCredentials: signingCredentials,
            expires: expirationTime
        );

        var token = new JwtSecurityTokenHandler().WriteToken(accessToken);

        return token;
    }

    private async Task InsertStudentLoginDetails(string ssoid, string? deviceRegistrationToken)
    {
        var studentEntity = new tblStudentLoginDetail
        {
            LoginTime = DateTime.Now,
            SSOID = ssoid,
            DeviceRegistrationToken = deviceRegistrationToken ?? ""
        };
                
        await _genericRepository.InsertAsync(studentEntity);
    }
}