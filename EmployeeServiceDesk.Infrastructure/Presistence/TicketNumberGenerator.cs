using Microsoft.EntityFrameworkCore;
using ServiceDesk.Application.Abstractions;

namespace ServiceDesk.Infrastructure.Persistence;

/// <summary>Uses a SQL Server SEQUENCE so numbers stay unique under concurrent requests.</summary>
public class TicketNumberGenerator : ITicketNumberGenerator
{
    private readonly AppDbContext _db;

    public TicketNumberGenerator(AppDbContext db) => _db = db;

    public async Task<string> NextAsync(CancellationToken ct = default)
    {
        var next = await _db.Database
            .SqlQueryRaw<int>($"SELECT NEXT VALUE FOR {AppDbContext.TicketNumberSequence} AS [Value]")
            .SingleAsync(ct);

        return $"TKT-{DateTime.UtcNow.Year}-{next:D6}";
    }
}
