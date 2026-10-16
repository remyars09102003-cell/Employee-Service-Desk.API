using EmployeeServiceDesk.Domain.Enums;

namespace EmployeeServiceDesk.Application.DTOs.Notification;

public class NotificationDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public NotificationType Type { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public int? TicketId { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }
}