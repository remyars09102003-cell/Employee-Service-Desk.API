using ServiceDesk.Domain.Entities;
using ServiceDesk.Domain.Enums;
using System.Net.Sockets;

namespace ServiceDesk.Application.Abstractions.Persistence;

public sealed record TicketFilter(
    string? Search,
    TicketStatus? Status,
    TicketPriority? Priority,
    int? CategoryId,
    int? ServiceItemId,
    int? RequesterEmployeeId,
    DateTime? CreatedFromUtc,
    DateTime? CreatedToExclusiveUtc);

public interface ITicketRepository
{
    /// <summary>Tracked entity including Category and ServiceItem, for updates.</summary>
    Task<Ticket?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<(IReadOnlyList<Ticket> Items, int TotalCount)> GetPagedAsync(
        TicketFilter filter, int page, int pageSize, CancellationToken ct = default);

    Task<IReadOnlyList<TicketHistory>> GetHistoryAsync(int ticketId, CancellationToken ct = default);

    Task AddAsync(Ticket ticket, CancellationToken ct = default);
}
