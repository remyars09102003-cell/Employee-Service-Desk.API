using EmployeeServiceDesk.Domain.Enums;

namespace EmployeeServiceDesk.API.Controllers;

public class CreateNotificationRequest
{
    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; }

    public int? TicketId { get; set; }
}