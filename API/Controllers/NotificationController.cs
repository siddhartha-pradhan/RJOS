using Application.DTOs.Notification;
using Application.Interfaces.Services;
using Common.Constants;
using Common.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace RJOS.Controllers;

public class NotificationController : BaseController<NotificationController>
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService, IWebHostEnvironment webHostEnvironment)
    {
        _notificationService = notificationService;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _notificationService.GetAllNotifications();
        
        return View(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetNotificationById(int notificationId)
    {
        var notification = await _notificationService.GetNotificationById(notificationId);

        return Json(new
        {
            data = notification
        });
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(NotificationRequestDTO notification)
    {
        if (string.IsNullOrEmpty(notification.Title))
        {
            return Json(new
            {
                errorType = 1
            });
        }

        var action = 0;
        
        if (notification.UploadedFile != null)
        {
            var notificationDocumentPath = DocumentUploadFilePath.NotificationDocumentFilePath;
            
            var serverDocName = await UploadDocument(notificationDocumentPath, notification.UploadedFile);

            notification.UploadedFileUrl = serverDocName;
            notification.UploadedFileName = notification.UploadedFile.FileName;
        }

        if(notification.Id != 0)
        {
            action = 1;
            await _notificationService.UpdateNotification(notification);
        }
        else
        {
            action = 2;
            await _notificationService.InsertNotification(notification);
        }
        
        var result = await _notificationService.GetAllNotifications();

        return Json(new
        {
            action = action,
            htmlData = ConvertViewToString("_NotificationsList", result, true)
        });
    }
    
    public IActionResult DownloadDocument(string fileName, string uploadFileName)
    {
        var filePath = DocumentUploadFilePath.NotificationDocumentFilePath;

        var sPhysicalPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath + fileName);

        return !System.IO.File.Exists(sPhysicalPath) ? Content($"file not found.") : DownloadAnyFile(uploadFileName, sPhysicalPath, null);
    }
    
    private async Task<string> UploadDocument(string folderPath, IFormFile file)
    {
        if (!Directory.Exists(Path.Combine(_webHostEnvironment.WebRootPath, folderPath)))
        {
            Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, folderPath));
        }

        var uploadedDocumentPath = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);
        
        var extension = Path.GetExtension(file.FileName);
        
        var fileName = extension.SetUniqueFileName();

        await using var stream = new FileStream(Path.Combine(uploadedDocumentPath, fileName), FileMode.Create);
            
        await file.CopyToAsync(stream);
            
        return fileName;
    }
}