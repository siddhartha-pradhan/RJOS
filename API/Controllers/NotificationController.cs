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

    [Authentication]
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
        var userId = HttpContext.Session.GetInt32("UserId");
        
        notification.UserId = userId ?? 1;

        if (string.IsNullOrEmpty(notification.Title))
        {
            return Json(new
            {
                errorType = 1
            });
        }

        var action = 0;
        var message = "";
        
        if (notification.UploadedFile != null)
        {
            if (!ValidateFileMimeType(notification.UploadedFile))
            {
                action = 0;
                message = $"The file format of {notification.UploadedFile.FileName} is incorrect.";

                return Json(new
                {
                    errorType = action,
                    message = message
                });
            }
            
            if (!IsValidFileName(notification.UploadedFile.FileName))
            {
                action = 0;
                message = $"The file name must not contain special characters. Only the following characters are allowed while naming the files, <br />1. a to z characters.<br/>2. numbers(0 to 9). <br />3. - and _ with space. <br /> The naming convention for {notification.UploadedFile.FileName} file is incorrect.";

                return Json(new
                {
                    errorType = action,
                    message = message
                });
            }

            var fileValidLength = IsFileLengthValid(notification.UploadedFile);
            
            if (!fileValidLength.Item1)
            {
                action = 0;
                message = $"The uploaded file exceeds 30 MB. The file size for {notification.UploadedFile.FileName} file is {fileValidLength.Item2} MB.";

                return Json(new
                {
                    errorType = action,
                    message = message
                });
            }
            
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
            message = message,
            htmlData = ConvertViewToString("_NotificationsList", result, true)
        });
    }

    [HttpPost]
    public async Task<IActionResult> TriggerNotification(int notificationId)
    {
        await _notificationService.NotifyNotification(notificationId);

        var notifications = await _notificationService.GetAllNotifications();
        
        return Json(new
        {
            data = "Push notification successfully triggered.",
            htmlData = ConvertViewToString("_NotificationsList", notifications, true)
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