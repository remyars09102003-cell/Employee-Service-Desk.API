namespace EmployeeServiceDesk.Application.DTOs.Asset;

public class AssignAssetDto
{
    public int EmployeeId { get; set; }

    public int? AssignedByUserId { get; set; }

    public string? Notes { get; set; }
}