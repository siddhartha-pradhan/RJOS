using Application.DTOs.Notification;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Data.Implementation.Services;

public class NotificationService : INotificationService
{
    private readonly IGenericRepository _genericRepository;

    public NotificationService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<List<NotificationResponseDTO>> GetAllNotifications()
    {
        var notifications = await _genericRepository.GetAsync<tblNotification>(x => x.IsActive);
        
        return notifications.Select(x => new NotificationResponseDTO
        {
            Id = x.Id,
            Title = x.Header,
            Description = x.Description,
            UploadedFileName = x.UploadedFileName,
            UploadedFileUrl = x.UploadedFileUrl,
            IsActive = x.IsActive,
            CreatedOn = x.CreatedOn,
        }).ToList();
    }

    public async Task<NotificationResponseDTO> GetNotificationById(int notificationId)
    {
        var notification = await _genericRepository.GetByIdAsync<tblNotification>(notificationId);

        if (notification != null)
        {
            return new NotificationResponseDTO()
            {
                Id = notification.Id,
                Title = notification.Header,
                Description = notification.Description,
            };
        }

        return new NotificationResponseDTO();
    }
    
    public async Task InsertNotification(NotificationRequestDTO notification)
    {
        var notificationModel = new tblNotification()
        {
            Header = notification.Title,
            Description = notification.Description,
            UploadedFileName = notification.UploadedFileName,
            UploadedFileUrl = notification.UploadedFileUrl,
            IsActive = true,
            CreatedBy = 1,
            CreatedOn = DateTime.Now
        };

        await _genericRepository.InsertAsync(notificationModel);
    }

    public async Task UpdateNotification(NotificationRequestDTO notification)
    {
        var notificationModel = await _genericRepository.GetByIdAsync<tblNotification>(notification.Id);

        if (notificationModel != null)
        {
            notificationModel.Header = notification.Title;
            notificationModel.Description = notification.Description;
            
            if (notification.UploadedFileUrl != null)
            {
                notificationModel.UploadedFileName = notification.UploadedFileName;
                notificationModel.UploadedFileUrl = notification.UploadedFileUrl;
            }

            await _genericRepository.UpdateAsync(notificationModel);
        }
    }
}