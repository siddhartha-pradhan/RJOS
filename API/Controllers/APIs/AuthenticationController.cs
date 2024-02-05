using Application.DTOs.Authentication;
using Application.DTOs.Base;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Authenticate(AuthenticationRequestDTO authenticationRequest)
    {
        var response = await _authenticationService.Authenticate(authenticationRequest);
            
        if (response.Id == 0)
        {
            var badRequest = new ResponseDTO<object>()
            {
                Status = "Bad Request",
                Message = "Please insert a valid ssoid and date of birth.",
                StatusCode = HttpStatusCode.BadRequest,
                Result = false
            };

            return BadRequest(badRequest);
        }

        var result = new ResponseDTO<AuthenticationResponseDTO>
        {
            StatusCode = HttpStatusCode.OK,
            Result = response,
            Message = "Successfully Logged In",
            Status = "Success."
        };

        return Ok(result);
    }
}