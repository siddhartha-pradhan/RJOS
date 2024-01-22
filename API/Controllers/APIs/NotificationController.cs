using System.Net;
using Application.DTOs.Base;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Notification;
using Application.Interfaces.Services;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/notifications")]
public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("get-valid-notifications")]
    public async Task<IActionResult> GetUserNotifications()
    {
        var result = await _notificationService.GetAllValidNotifications();
        
        var response = new ResponseDTO<List<NotificationResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
}