
using System.ComponentModel.DataAnnotations;

namespace EmployeeServiceDesk.Application.DTOs.Feedback;

public class UpdateFeedbackStatusDto
{
    [Required]
    [RegularExpression("^(Submitted|UnderReview|Resolved|Rejected)$")]
    public string Status { get; set; } = "Submitted";
}