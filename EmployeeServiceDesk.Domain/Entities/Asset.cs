using EmployeeServiceDesk.Domain.Enums;

namespace EmployeeServiceDesk.Domain.Entities;

public class Asset
{
    public int AssetId { get; set; }

    public string AssetTag { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? SerialNumber { get; set; }

    public int AssetTypeId { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public DateTime? WarrantyExpiryDate { get; set; }

    public AssetStatus Status { get; set; } = AssetStatus.Available;

    public int? AssignedEmployeeId { get; set; }

    public int? LinkedTicketId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    public AssetType? AssetType { get; set; }

    public ICollection<AssetAssignment> Assignments { get; set; }
        = new List<AssetAssignment>();
}