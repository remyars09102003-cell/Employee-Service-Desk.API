using EmployeeServiceDesk.Application.DTOs.Audit;
using EmployeeServiceDesk.Application.ServiceInterface;
using EmployeeServiceDesk.Domain.RepositoryInterface;

namespace EmployeeServiceDesk.Application.Services;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _auditRepository;

    public AuditService(IAuditRepository auditRepository)
    {
        _auditRepository = auditRepository;
    }

    public async Task<IReadOnlyList<AuditLogDto>> GetRecentAsync(
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        take = Math.Clamp(take, 1, 200);

        var logs = await _auditRepository.GetRecentAsync(
            take, cancellationToken);

        return logs.Select(x => new AuditLogDto
        {
            AuditLogId = x.AuditLogId,
            Action = x.Action,
            EntityName = x.EntityName,
            EntityId = x.EntityId,
            PerformedBy = x.PerformedBy,
            Details = x.Details,
            OccurredAtUtc = x.OccurredAtUtc
        }).ToList();
    }
}