﻿using Application.DTOs.Authentication;
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

    public AuthenticationService(IOptions<JwtSettings> jwtSettings, IOptions<RsosSettings> rsosSettings, IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
        _jwtSettings = jwtSettings.Value;
        _rsosSettings = rsosSettings.Value;   
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

        if (!response.IsSuccessStatusCode) return new AuthenticationResponseDTO();
        
        var responseData = await response.Content.ReadAsStringAsync();

        var apiResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(responseData);

        if (apiResponse is not { Status: true }) return new AuthenticationResponseDTO();
        
        var studentLoginData = apiResponse.Data.Student;

        var studentEntity = new tblStudentLoginDetail
        {
            LoginTime = DateTime.Now,
            SSOID = studentLoginData.SsoId,
            DeviceRegistrationToken = authenticationRequest.DeviceRegistrationToken ?? ""
        };
                
        await _genericRepository.InsertAsync(studentEntity);

        var pcpDates =  await _genericRepository.GetAsync<tblPcpDates>();

        var maxPcpDate = pcpDates.MaxBy(x => x.Id);

        var authenticationResponse = new AuthenticationResponseDTO
        {
            Id = studentLoginData.Id,
            Enrollment = studentLoginData.Enrollment,
            Name = studentLoginData.Name,
            DateOfBirth = studentLoginData.Dob.ToString("yyyy-MM-dd"),
            SSOID = studentLoginData.SsoId,
            ApplicationToken = GenerateJwtToken(studentLoginData),
            SecureRSOSToken = apiResponse.secure_token,
            ValidTill = apiResponse.secure_token_valid_till,
            StartDate = maxPcpDate != null ? maxPcpDate.StartDate.ToString("yyyy-MM-dd") : "",
            EndDate = maxPcpDate != null ? maxPcpDate!.EndDate.ToString("yyyy-MM-dd") : ""
        };

        return authenticationResponse;
    }

    public async Task<AuthenticationResponseDTO> AuthenticateForceLogin(AuthenticationForceLoginRequestDTO authenticationLoginRequest)
    {
        var httpClient = new HttpClient();

        var rsosToken = _rsosSettings.Token;

        var rsosUrl = _rsosSettings.URL;

        var baseUrl = $"{rsosUrl}/new_api_student_login";

        var queryParams = new System.Collections.Specialized.NameValueCollection
        {
            { "ssoid", authenticationLoginRequest.SSOID },
            { "dob", authenticationLoginRequest.DateOfBirth },
            { "token", rsosToken }
        };

        var uriBuilder = new UriBuilder(baseUrl)
        {
            Query = string.Join("&", Array.ConvertAll(queryParams.AllKeys,
                key => $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(queryParams[key])}"))
        };

        var postData = new StringContent("{\"key\": \"value\"}", Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(uriBuilder.Uri, postData);

        if (!response.IsSuccessStatusCode) return new AuthenticationResponseDTO();
        
        var responseData = await response.Content.ReadAsStringAsync();

        var apiResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(responseData);

        if (apiResponse is not { Status: true }) return new AuthenticationResponseDTO();
        
        var studentLoginData = apiResponse.Data.Student;

        var studentEntity = new tblStudentLoginDetail
        {
            LoginTime = DateTime.Now,
            SSOID = studentLoginData.SsoId,
            DeviceRegistrationToken = authenticationLoginRequest.DeviceRegistrationToken ?? ""
        };
                
        await _genericRepository.InsertAsync(studentEntity);

        var pcpDates =  await _genericRepository.GetAsync<tblPcpDates>();

        var maxPcpDate = pcpDates.MaxBy(x => x.Id);

        var secureToken = authenticationLoginRequest.IsForceLogIn == 1 ? apiResponse.secure_token : "";
        
        var authenticationResponse = new AuthenticationResponseDTO
        {
            Id = studentLoginData.Id,
            Enrollment = studentLoginData.Enrollment,
            Name = studentLoginData.Name,
            DateOfBirth = studentLoginData.Dob.ToString("yyyy-MM-dd"),
            SSOID = studentLoginData.SsoId,
            ApplicationToken = GenerateJwtToken(studentLoginData),
            SecureRSOSToken = secureToken,
            ValidTill = apiResponse.secure_token_valid_till,
            StartDate = maxPcpDate != null ? maxPcpDate.StartDate.ToString("yyyy-MM-dd") : "",
            EndDate = maxPcpDate != null ? maxPcpDate!.EndDate.ToString("yyyy-MM-dd") : ""
        };

        return authenticationResponse;

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