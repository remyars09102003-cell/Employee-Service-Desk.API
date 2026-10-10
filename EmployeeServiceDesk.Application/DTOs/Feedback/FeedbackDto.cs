
namespace EmployeeServiceDesk.Application.DTOs.Feedback;

public class FeedbackDto
{
    public int FeedbackId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string? EmployeeEmail { get; set; }

    public string Category { get; set; } = string.Empty;

    public int Rating { get; set; }

    public string Comments { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}