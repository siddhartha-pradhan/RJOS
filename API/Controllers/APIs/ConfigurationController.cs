using System.Net;
using Application.DTOs.Base;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/configuration")]
public class ConfigurationController : Controller
{
    private readonly IConfigurationService _configuration;

    public ConfigurationController(IConfigurationService configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("get-firebase-config")]
    public IActionResult GetFirebaseConfiguration()
    {
        var firebaseConfig = _configuration.GetFirebaseConfiguration();

        var result = new ResponseDTO<string>
        {
            StatusCode = HttpStatusCode.OK,
            Result = firebaseConfig,
            Message = "Firebase Configuration Successfully Retrieved",
            Status = "Success."
        };

        return Ok(result);
    }
}