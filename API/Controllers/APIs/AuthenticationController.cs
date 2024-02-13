using Application.DTOs.Authentication;
using Application.DTOs.Base;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/authentication")]
[IgnoreAntiforgeryToken]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromForm]AuthenticationRequestDTO authenticationRequest)
    {
        var response = await _authenticationService.Authenticate(authenticationRequest);

        switch (response.Id)
        {
            case 0:
            {
                var badRequest = new ResponseDTO<object>()
                {
                    Status = "Bad Request",
                    Message = "Please insert a valid SSOID and Date of Birth.",
                    StatusCode = HttpStatusCode.BadRequest,
                    Result = false
                };

                return BadRequest(badRequest);
            }
            case -1:
            {
                var unauthorized = new ResponseDTO<object>()
                {
                    Status = "Unauthorized",
                    Message = $"You account has been locked due to 5 attempts for invalid password, you can now try again after {response.ValidTill}",
                    StatusCode = HttpStatusCode.Unauthorized,
                    Result = false
                };
            
                return Unauthorized(unauthorized);
            }
            default:
            {
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
    }
}