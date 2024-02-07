using Application.DTOs.NewsAndAlert;
using Application.DTOs.PcpDate;
using Application.Interfaces.Services;
using Common.Utilities;
using Data.Implementation.Services;
using Microsoft.AspNetCore.Mvc;

namespace RJOS.Controllers;

public class PcpDatesController : BaseController<PcpDatesController>
{
    private readonly IPcpDatesService _pcpDatesService;

    public PcpDatesController(IPcpDatesService pcpDatesService)
    {
        _pcpDatesService = pcpDatesService;
    }

    [Authentication]
    public async Task<IActionResult> Index()
    {
        var result = await _pcpDatesService.GetAllPcpDates();

        return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Insert(PcpDatesRequestDTO pcpDatesRequest)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        pcpDatesRequest.UserId = userId ?? 1;


        if (pcpDatesRequest.UserId != 0)
        {
            await _pcpDatesService.InsertPcpDates(pcpDatesRequest);
        }

        var result = await _pcpDatesService.GetAllPcpDates();

        return Json(new
        {
            htmlData = ConvertViewToString("_pcpDatesList", result, true)
        });
    }

    //[HttpGet]
    //public async Task<IActionResult> GetAllPcpDates()
    //{
    //    var result = await _pcpDatesService.GetAllPcpDates();
    //    return result;
    //}
}
