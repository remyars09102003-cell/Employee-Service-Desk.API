namespace EmployeeServiceDesk.Domain.Entities
{
    public class Location
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Employee> Employees { get; set; }
            = new List<Employee>();
    }
}