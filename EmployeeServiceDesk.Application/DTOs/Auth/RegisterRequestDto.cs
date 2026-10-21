using System.ComponentModel.DataAnnotations;

namespace EmployeeServiceDesk.Application.DTOs.Auth
{
    public class RegisterRequestDto
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Employee";
    }
}