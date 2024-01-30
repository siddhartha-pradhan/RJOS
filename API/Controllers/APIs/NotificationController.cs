using System.Net;
using Application.DTOs.Base;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Notification;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.StaticFiles;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/notifications")]
public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public NotificationController(INotificationService notificationService, IWebHostEnvironment webHostEnvironment)
    {
        _notificationService = notificationService;
        _webHostEnvironment = webHostEnvironment;
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
    
    [HttpPost("send-notification/{registrationToken}")]
    public async Task<IActionResult> SendNotification(string registrationToken)
    {
        await _notificationService.NotifyNotification(registrationToken);
        
        var response = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Notified.",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(response);
    }
    
    [HttpGet("download-notification-attachment/{notificationId}")]
    public async Task<IActionResult> DownloadNotificationAttachment(int notificationId)
    {
        var notification = await _notificationService.GetNotificationById(notificationId);

        if (string.IsNullOrEmpty(notification.UploadedFileUrl))
        {
            var notFound = new ResponseDTO<object>()
            {
                Status = "Not Found",
                Message = "Attachment Not Found.",
                StatusCode = HttpStatusCode.NotFound,
                Result = false
            };
            
            return NotFound(notFound);
        }
        
        var wwwRootPath = _webHostEnvironment.WebRootPath;
        
        var documentsFolderPath = Path.Combine(wwwRootPath, "documents");

        var notificationsFolderPath = Path.Combine(documentsFolderPath, "notifications");
        
        var filePath = Path.Combine(notificationsFolderPath, notification.UploadedFileUrl);

        if (!System.IO.File.Exists(filePath))
        {
            var notFound = new ResponseDTO<object>()
            {
                Status = "Not Found",
                Message = "Attachment Not Found.",
                StatusCode = HttpStatusCode.NotFound,
                Result = false
            };
            
            return NotFound(notFound);
        }
        
        var memory = new MemoryStream();
        
        await using(var stream = new FileStream(filePath, FileMode.Open)) 
        {
            await stream.CopyToAsync(memory);
        }
        
        memory.Position = 0;
        
        return File(memory, GetContentType(filePath), notification.UploadedFileName);
    }
    
    private static string GetContentType(string path) {
        var provider = new FileExtensionContentTypeProvider();
        
        if (!provider.TryGetContentType(path, out var contentType)) {
            contentType = "application/octet-stream";
        }
        
        return contentType;
    }
}