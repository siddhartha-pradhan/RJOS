using Application.DTOs.Notification;

namespace Application.Interfaces.Services;

public interface INotificationService
{
    Task<List<NotificationResponseDTO>> GetAllNotifications();

    Task<NotificationResponseDTO> GetNotificationById(int notificationId);

    Task InsertNotification(NotificationRequestDTO notification);

    Task UpdateNotification(NotificationRequestDTO notification);
}