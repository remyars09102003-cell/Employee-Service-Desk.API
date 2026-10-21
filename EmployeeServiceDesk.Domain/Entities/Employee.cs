using EmployeeServiceDesk.Domain.Enums;

namespace EmployeeServiceDesk.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        public string EmployeeId { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Designation { get; set; }

        public DateTime JoiningDate { get; set; }

        public EmploymentStatus EmploymentStatus { get; set; }
            = EmploymentStatus.Active;

        public int DepartmentId { get; set; }

        public Department? Department { get; set; }

        public int? ManagerId { get; set; }

        public Employee? Manager { get; set; }

        public ICollection<Employee> Subordinates { get; set; }
            = new List<Employee>();

        public bool IsActive { get; set; } = true;
    }
}