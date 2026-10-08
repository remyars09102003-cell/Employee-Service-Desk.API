namespace EmployeeServiceDesk.Domain.Entities;

public class AssetAssignment
{
    public int AssetAssignmentId { get; set; }

    public int AssetId { get; set; }

    public int EmployeeId { get; set; }

    public int? AssignedByUserId { get; set; }

    public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? ReturnedAtUtc { get; set; }

    public string? Notes { get; set; }

    public Asset? Asset { get; set; }
}