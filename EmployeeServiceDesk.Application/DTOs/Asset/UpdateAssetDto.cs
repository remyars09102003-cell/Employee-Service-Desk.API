namespace EmployeeServiceDesk.Application.DTOs.Asset;

public class UpdateAssetDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? SerialNumber { get; set; }

    public int AssetTypeId { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public DateTime? WarrantyExpiryDate { get; set; }

    public bool IsActive { get; set; }
}