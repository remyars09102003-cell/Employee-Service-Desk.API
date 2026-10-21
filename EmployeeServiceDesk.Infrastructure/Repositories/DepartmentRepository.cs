using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.RepositoryInterface;
using EmployeeServiceDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeServiceDesk.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<Department>> GetAllAsync(
            CancellationToken ct = default)
        {
            return _context.Departments
                .Include(d => d.Employees)
                .OrderBy(d => d.Name)
                .ToListAsync(ct);
        }

        public Task<Department?> GetByIdAsync(
            int id,
            CancellationToken ct = default)
        {
            return _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id, ct);
        }

        public Task<bool> NameExistsAsync(
            string name,
            int? exceptId = null,
            CancellationToken ct = default)
        {
            return _context.Departments.AnyAsync(
                d => d.Name == name &&
                     (!exceptId.HasValue || d.Id != exceptId.Value),
                ct);
        }

        public async Task AddAsync(
            Department department,
            CancellationToken ct = default)
        {
            await _context.Departments.AddAsync(department, ct);
        }

        public void Update(Department department)
        {
            _context.Departments.Update(department);
        }

        public async Task SaveChangesAsync(
            CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}