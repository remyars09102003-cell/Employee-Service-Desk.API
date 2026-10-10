using EmployeeServiceDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeServiceDesk.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Asset Management
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AssetType> AssetTypes => Set<AssetType>();
    public DbSet<AssetAssignment> AssetAssignments => Set<AssetAssignment>();

    // Feedback Management
    public DbSet<Feedback> FeedbackEntries => Set<Feedback>();

    // Audit Management
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Asset Type Configuration
        modelBuilder.Entity<AssetType>(entity =>
        {
            entity.HasKey(x => x.AssetTypeId);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.HasMany(x => x.Assets)
                .WithOne(x => x.AssetType)
                .HasForeignKey(x => x.AssetTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Asset Configuration
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(x => x.AssetId);

            entity.HasIndex(x => x.AssetTag)
                .IsUnique();

            entity.Property(x => x.AssetTag)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Description)
                .HasMaxLength(1000);

            entity.Property(x => x.SerialNumber)
                .HasMaxLength(100);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30);

            entity.HasMany(x => x.Assignments)
                .WithOne(x => x.Asset)
                .HasForeignKey(x => x.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Asset Assignment Configuration
        modelBuilder.Entity<AssetAssignment>(entity =>
        {
            entity.HasKey(x => x.AssetAssignmentId);

            entity.Property(x => x.Notes)
                .HasMaxLength(500);

            entity.HasIndex(x => new
            {
                x.AssetId,
                x.EmployeeId
            });
        });

        // Feedback Configuration
        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(x => x.FeedbackId);

            entity.Property(x => x.EmployeeName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.EmployeeEmail)
                .HasMaxLength(256);

            entity.Property(x => x.Category)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Rating)
                .IsRequired();

            entity.Property(x => x.Comments)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(x => x.CreatedAtUtc)
                .IsRequired();
        });

        // Audit Log Configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(x => x.AuditLogId);

            entity.Property(x => x.Action)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.EntityName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.EntityId)
                .HasMaxLength(100);

            entity.Property(x => x.PerformedBy)
                .HasMaxLength(256);

            entity.Property(x => x.Details)
                .HasMaxLength(2000);

            entity.Property(x => x.OccurredAtUtc)
                .IsRequired();

            entity.HasIndex(x => x.OccurredAtUtc);
        });
    }
}
