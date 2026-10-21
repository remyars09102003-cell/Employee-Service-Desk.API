using EmployeeServiceDesk.Domain.Entities;

namespace EmployeeServiceDesk.Domain.RepositoryInterface
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync(
            CancellationToken ct = default);

        Task<Employee?> GetByIdAsync(
            int id,
            CancellationToken ct = default);

        Task<Employee?> GetByEmployeeIdAsync(
            string employeeId,
            CancellationToken ct = default);

        Task AddAsync(
            Employee employee,
            CancellationToken ct = default);

        Task UpdateAsync(
            Employee employee,
            CancellationToken ct = default);

        Task DeleteAsync(
            Employee employee,
            CancellationToken ct = default);

        Task SaveChangesAsync(
            CancellationToken ct = default);
    }
}