using Application.DTOs.Base;
using Application.DTOs.NewsAndAlert;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/news-and-alerts")]

public class NewsAndAlertController : Controller
{
    private readonly INewsAndAlertService _newsAndAlertService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public NewsAndAlertController(INewsAndAlertService newsAndAlertService, IWebHostEnvironment webHostEnvironment)
    {
        _newsAndAlertService = newsAndAlertService;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("get-valid-news-and-alerts")]
    public async Task<IActionResult> GetUserNewsAndAlerts()
    {
        var result = await _newsAndAlertService.GetAllValidNewsAndAlert();

        var response = new ResponseDTO<List<NewsAndAlertResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [HttpPost("get-valid-news-and-alerts")]
    public async Task<IActionResult> PostUserNewsAndAlerts()
    {
        var result = await _newsAndAlertService.GetAllValidNewsAndAlert();

        var response = new ResponseDTO<List<NewsAndAlertResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
}
