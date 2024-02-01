using Application.DTOs.Authentication;

namespace Application.Interfaces.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResponseDTO> Authenticate(AuthenticationRequestDTO authenticationRequest);
}