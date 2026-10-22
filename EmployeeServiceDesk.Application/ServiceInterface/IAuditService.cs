using EmployeeServiceDesk.Application.DTOs.Audit;

namespace EmployeeServiceDesk.Application.ServiceInterface;

public interface IAuditService
{
    Task<IReadOnlyList<AuditLogDto>> GetRecentAsync(
        int take = 50,
        CancellationToken cancellationToken = default);
}