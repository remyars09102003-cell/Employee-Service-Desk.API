using Microsoft.EntityFrameworkCore;
using ServiceDesk.Application.Abstractions.Persistence;
using ServiceDesk.Domain.Common;
using ServiceDesk.Domain.Entities;
using System.Net.Sockets;

namespace ServiceDesk.Infrastructure.Persistence;

/// <summary>
/// Shared by all members. Each member adds their own DbSets below; entity mappings are
/// auto-discovered from the Configurations folder, so there are no merge conflicts in OnModelCreating.
/// </summary>
public class AppDbContext : DbContext, IUnitOfWork
{
    public const string TicketNumberSequence = "TicketNumberSeq";

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ---- Member 2: Ticket Operations
    public DbSet<TicketCategory> TicketCategories => Set<TicketCategory>();
    public DbSet<ServiceItem> ServiceItems => Set<ServiceItem>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketHistory> TicketHistory => Set<TicketHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Member 2: database sequence behind ticket numbers (TKT-2026-000123)
        modelBuilder.HasSequence<int>(TicketNumberSequence).StartsAt(1).IncrementsBy(1);

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>()
                     .Where(e => e.State == EntityState.Modified))
        {
            entry.Entity.UpdatedAtUtc = now;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
