using EmployeeServiceDesk.Domain.Enums;

namespace EmployeeServiceDesk.Application.DTOs.SLA;

public class UpdateSlaDto
{
    public string Name { get; set; } = string.Empty;

    public Priority Priority { get; set; }

    public int ResponseTimeMinutes { get; set; }

    public int ResolutionTimeMinutes { get; set; }

    public bool IsActive { get; set; }
}