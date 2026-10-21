using EmployeeServiceDesk.Domain.Entities;

namespace EmployeeServiceDesk.Domain.RepositoryInterface
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllAsync(
            CancellationToken ct = default);

        Task<Department?> GetByIdAsync(
            int id,
            CancellationToken ct = default);

        Task<bool> NameExistsAsync(
            string name,
            int? exceptId = null,
            CancellationToken ct = default);

        Task AddAsync(
            Department department,
            CancellationToken ct = default);

        void Update(Department department);

        Task SaveChangesAsync(
            CancellationToken ct = default);
    }
}