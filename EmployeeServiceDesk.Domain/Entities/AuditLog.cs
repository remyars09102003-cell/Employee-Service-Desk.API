
namespace EmployeeServiceDesk.Domain.Entities;

public class AuditLog
{
    public long AuditLogId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public string? EntityId { get; set; }

    public string? PerformedBy { get; set; }

    public string? Details { get; set; }

    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
}