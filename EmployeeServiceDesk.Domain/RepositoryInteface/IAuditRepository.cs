using EmployeeServiceDesk.Domain.Entities;

namespace EmployeeServiceDesk.Domain.RepositoryInterface;

public interface IAuditRepository
{
    Task AddAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken = default);

    Task<List<AuditLog>> GetRecentAsync(
        int take,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}