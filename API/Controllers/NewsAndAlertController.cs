using Application.DTOs.NewsAndAlert;
using Application.Interfaces.Services;
using Common.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace RJOS.Controllers
{
    public class NewsAndAlertController : BaseController<NewsAndAlertController>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INewsAndAlertService _newsAndAlertService;

        public NewsAndAlertController(INewsAndAlertService newsAndAlertService, IWebHostEnvironment webHostEnvironment)
        {
            _newsAndAlertService = newsAndAlertService;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authentication]
        public async Task<IActionResult> Index()
        {
            var result = await _newsAndAlertService.GetAllNewsAndAlert();

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetNewsAndAlertById(int newsAndAlertId)
        {
            var newsAndAlert = await _newsAndAlertService.GetNewsAndAlertById(newsAndAlertId);

            return Json(new
            {
                data = newsAndAlert
            });
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(NewsAndAlerRequestDTO newsAndAlert)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            newsAndAlert.UserId = userId ?? 1;

            if (string.IsNullOrEmpty(newsAndAlert.Header))
            {
                return Json(new
                {
                    errorType = 1
                });
            }

            var action = 0;

            if (newsAndAlert.Id != 0)
            {
                action = 1;
                await _newsAndAlertService.UpdateNewsAndAlert(newsAndAlert);
            }
            else
            {
                action = 2;
                await _newsAndAlertService.InsertNewsAndAlert(newsAndAlert);
            }

            var result = await _newsAndAlertService.GetAllNewsAndAlert();

            return Json(new
            {
                action = action,
                htmlData = ConvertViewToString("_NewsAndAlertList", result, true)
            });
        }



    }
}
