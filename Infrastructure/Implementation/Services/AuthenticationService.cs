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
    private readonly IGenericRepository _genericRepository;
    private readonly IConfiguration _configuration;
    private readonly JwtSettings _jwtSettings;
    private readonly RsosSettings _rsosSettings;

    public AuthenticationService(IGenericRepository genericRepository, IConfiguration configuration, IOptions<JwtSettings> jwtSettings, IOptions<RsosSettings> rsosSettings)
    {
        _genericRepository = genericRepository;
        _configuration = configuration;
        _jwtSettings = jwtSettings.Value;
        _rsosSettings = rsosSettings.Value;   
    }

    public async Task<AuthenticationResponseDTO> Authenticate(AuthenticationRequestDTO authenticationRequest)
    {
        var httpClient = new HttpClient();

        var rsosToken = _rsosSettings.RSOS_Token;

        var rsosUrl = _rsosSettings.RSOS_URL;

        var baseUrl = $"{rsosUrl}/api_student_login";

        var queryParams = new System.Collections.Specialized.NameValueCollection
        {
            { "ssoid", authenticationRequest.SSOID },
            { "dob", authenticationRequest.DateOfBirth },
            { "token", rsosToken }
        };

        var uriBuilder = new UriBuilder(baseUrl);
        
        uriBuilder.Query = string.Join("&", Array.ConvertAll(queryParams.AllKeys,
                                        key => $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(queryParams[key])}"));

        var postData = new StringContent("{\"key\": \"value\"}", Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(uriBuilder.Uri, postData);

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(responseData);

            if (apiResponse is { Status: true })
            {
                var studentLoginData = apiResponse.Data.Student;

                // var studentEntity = new tblStudentLoginDetail
                // {
                //     LoginTime = DateTime.Now,
                //     SSOID = studentLoginData.SsoId,
                //     DeviceRegistrationToken = authenticationRequest.DeviceRegistrationToken
                // };
                //
                // await _genericRepository.InsertAsync(studentEntity);

                var authenticationResponse = new AuthenticationResponseDTO
                {
                    Id = studentLoginData.Id,
                    Enrollment = studentLoginData.Enrollment,
                    Name = studentLoginData.Name,
                    DateOfBirth = studentLoginData.Dob,
                    SSOID = studentLoginData.SsoId,
                    JWT = GenerateJwtToken(studentLoginData)
                };

                return authenticationResponse;
            }
            else
            {
                return new AuthenticationResponseDTO();
            }
        } else
        {
            return new AuthenticationResponseDTO();
        }
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
            new("enrollment", studentInfo.Enrollment.ToString()),
            new("ssoid", studentInfo.SsoId.ToString()),
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
}