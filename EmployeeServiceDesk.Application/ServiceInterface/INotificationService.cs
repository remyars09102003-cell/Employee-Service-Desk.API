using EmployeeServiceDesk.Application.DTOs.Notification;

namespace EmployeeServiceDesk.Application.Interfaces;

public interface INotificationService
{
    Task<List<NotificationDto>> GetByUserIdAsync(int userId);

    Task<NotificationDto> CreateAsync(
        int userId,
        string email,
        string title,
        string message,
        Domain.Enums.NotificationType type,
        int? ticketId);

    Task<bool> MarkAsReadAsync(int id);
}