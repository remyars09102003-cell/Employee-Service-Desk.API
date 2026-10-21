using Microsoft.EntityFrameworkCore;
using ServiceDesk.Application.Abstractions.Persistence;
using ServiceDesk.Domain.Entities;
using System.Net.Sockets;

namespace ServiceDesk.Infrastructure.Persistence.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _db;

    public TicketRepository(AppDbContext db) => _db = db;

    public Task<Ticket?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Tickets
            .Include(t => t.Category)
            .Include(t => t.ServiceItem)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<(IReadOnlyList<Ticket> Items, int TotalCount)> GetPagedAsync(
        TicketFilter filter, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.Tickets
            .AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.ServiceItem)
            .AsQueryable();

        if (filter.RequesterEmployeeId.HasValue)
            query = query.Where(t => t.RequesterEmployeeId == filter.RequesterEmployeeId.Value);
        if (filter.Status.HasValue)
            query = query.Where(t => t.Status == filter.Status.Value);
        if (filter.Priority.HasValue)
            query = query.Where(t => t.Priority == filter.Priority.Value);
        if (filter.CategoryId.HasValue)
            query = query.Where(t => t.CategoryId == filter.CategoryId.Value);
        if (filter.ServiceItemId.HasValue)
            query = query.Where(t => t.ServiceItemId == filter.ServiceItemId.Value);
        if (filter.CreatedFromUtc.HasValue)
            query = query.Where(t => t.CreatedAtUtc >= filter.CreatedFromUtc.Value);
        if (filter.CreatedToExclusiveUtc.HasValue)
            query = query.Where(t => t.CreatedAtUtc < filter.CreatedToExclusiveUtc.Value);
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(t => t.TicketNumber.Contains(filter.Search) || t.Title.Contains(filter.Search));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(t => t.CreatedAtUtc)
            .ThenByDescending(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<IReadOnlyList<TicketHistory>> GetHistoryAsync(int ticketId, CancellationToken ct = default)
        => await _db.TicketHistory
            .AsNoTracking()
            .Where(h => h.TicketId == ticketId)
            .OrderBy(h => h.ChangedAtUtc).ThenBy(h => h.Id)
            .ToListAsync(ct);

    public async Task AddAsync(Ticket ticket, CancellationToken ct = default)
        => await _db.Tickets.AddAsync(ticket, ct);
}
