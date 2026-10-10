
namespace EmployeeServiceDesk.Domain.Entities;

public class Feedback
{
    public int FeedbackId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string? EmployeeEmail { get; set; }

    public string Category { get; set; } = string.Empty;

    public int Rating { get; set; }

    public string Comments { get; set; } = string.Empty;

    public string Status { get; set; } = "Submitted";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }
}