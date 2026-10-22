namespace EmployeeServiceDesk.Application.DTOs.Asset;

public class CreateAssetDto
{
    public string AssetTag { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? SerialNumber { get; set; }

    public int AssetTypeId { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public DateTime? WarrantyExpiryDate { get; set; }
}