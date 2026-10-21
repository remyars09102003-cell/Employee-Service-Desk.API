namespace EmployeeServiceDesk.Application.DTOs.Employees
{
    public class EmployeeDto
    {
        public int Id { get; set; }

        public string EmployeeCode { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}