using ServiceDesk.Application.Common;
using ServiceDesk.Application.Tickets.Dtos;

namespace EmployeeServiceDesk.Application.Abstractions.Tickets.Services;

public interface ITicketService
{
    Task<PagedResult<TicketSummaryResponse>> GetPagedAsync(TicketQuery query, CancellationToken ct = default);
    Task<TicketResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TicketResponse> CreateAsync(CreateTicketRequest request, CancellationToken ct = default);
    Task<TicketResponse> UpdateAsync(int id, UpdateTicketRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<TicketHistoryResponse>> GetHistoryAsync(int id, CancellationToken ct = default);
}
