using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.RepositoryInterface;
using EmployeeServiceDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeServiceDesk.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync(
            CancellationToken ct = default)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Manager)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<Employee?> GetByIdAsync(
            int id,
            CancellationToken ct = default)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public async Task<Employee?> GetByEmployeeIdAsync(
            string employeeId,
            CancellationToken ct = default)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(
                    e => e.EmployeeId == employeeId,
                    ct);
        }

        public async Task AddAsync(
            Employee employee,
            CancellationToken ct = default)
        {
            await _context.Employees.AddAsync(employee, ct);
        }

        public async Task UpdateAsync(
            Employee employee,
            CancellationToken ct = default)
        {
            _context.Employees.Update(employee);

            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(
            Employee employee,
            CancellationToken ct = default)
        {
            _context.Employees.Remove(employee);

            await _context.SaveChangesAsync(ct);
        }

        public async Task SaveChangesAsync(
            CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}