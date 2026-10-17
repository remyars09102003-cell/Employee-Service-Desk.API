
using System.ComponentModel.DataAnnotations;

namespace EmployeeServiceDesk.Application.DTOs.Feedback;

public class CreateFeedbackDto
{
    [Required]
    [StringLength(100)]
    public string EmployeeName { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(256)]
    public string? EmployeeEmail { get; set; }

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [StringLength(2000)]
    public string Comments { get; set; } = string.Empty;
}