using Application.DTOs.Authentication;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Data.Implementation.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IGenericRepository _genericRepository;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public AuthenticationService(IGenericRepository genericRepository, IConfiguration configuration, HttpClient httpClient)
    {
        _genericRepository = genericRepository;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<AuthenticationResponseDTO> Authenticate(AuthenticationRequestDTO authenticationRequest)
    {
        throw new NotImplementedException();
    }
}