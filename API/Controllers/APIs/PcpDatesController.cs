using Application.DTOs.Base;
using Application.DTOs.NewsAndAlert;
using Application.DTOs.PcpDate;
using Application.DTOs.Student;
using Application.Interfaces.Services;
using Data.Implementation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace RJOS.Controllers.APIs;

[Route("api/pcp-dates")]
[ApiController]
public class PcpDatesController : ControllerBase
{
    private readonly IPcpDatesService _pcpDatesService;

    public PcpDatesController(IPcpDatesService pcpDatesService)
    {
        _pcpDatesService = pcpDatesService;
    }

    [HttpGet("get-pcp-Dates")]
    public async Task<IActionResult> GetAllPcpDates()
    {
        var result = await _pcpDatesService.GetAllPcpDates();

        var response = new ResponseDTO<List<PcpDatesResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
}
