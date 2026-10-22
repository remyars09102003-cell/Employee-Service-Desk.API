namespace EmployeeServiceDesk.Application.DTOs.Asset;

public class AssetDto
{
    public int AssetId { get; set; }

    public string AssetTag { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? SerialNumber { get; set; }

    public int AssetTypeId { get; set; }

    public string? AssetTypeName { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public DateTime? WarrantyExpiryDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public int? AssignedEmployeeId { get; set; }

    public bool IsActive { get; set; }
}