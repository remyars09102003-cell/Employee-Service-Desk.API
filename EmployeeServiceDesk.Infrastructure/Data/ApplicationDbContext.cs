using EmployeeServiceDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace EmployeeServiceDesk.Infrastructure.Data;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {
    }

    // Asset Management
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AssetType> AssetTypes => Set<AssetType>();
    public DbSet<AssetAssignment> AssetAssignments => Set<AssetAssignment>();

    // Feedback Management
    public DbSet<Feedback> FeedbackEntries => Set<Feedback>();
    public DbSet<SLA> SLAs =>Set<SLA>();

    // Audit Management
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<KnowledgeArticle>KnowledgeArticles =>Set<KnowledgeArticle>();
    public DbSet<Notification>Notifications =>Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Asset Type Configuration
        modelBuilder.Entity<AssetType>(entity =>
        modelBuilder.Entity<SLA>(entity =>
        {
            entity.HasKey(x => x.AssetTypeId);
            entity.HasKey(x => x.Id);

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
            entity.Property(x => x.Priority)
                .IsRequired();

            entity.Property(x => x.ResponseTimeMinutes)
                .IsRequired();
            entity.Property(x => x.SerialNumber)
                .HasMaxLength(100);

            entity.Property(x => x.ResolutionTimeMinutes)
                .IsRequired();
            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30);

            entity.HasMany(x => x.Assignments)
                .WithOne(x => x.Asset)
                .HasForeignKey(x => x.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(x => x.IsActive)
                .IsRequired();
        });

        // Asset Assignment Configuration
        modelBuilder.Entity<AssetAssignment>(entity =>
        modelBuilder.Entity<KnowledgeArticle>(entity =>
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
            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(250);
                .HasMaxLength(100);

            entity.Property(x => x.EmployeeEmail)
                .HasMaxLength(256);

            entity.Property(x => x.Category)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Rating)
            entity.Property(x => x.Content)
                .IsRequired();

            entity.Property(x => x.Tags)
                .HasMaxLength(500);
            entity.Property(x => x.Comments)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(x => x.Status)
                .IsRequired();
                .IsRequired()
                .HasMaxLength(30);

            entity.HasIndex(x => x.Title);
            entity.HasIndex(x => x.CategoryId);
            entity.HasIndex(x => x.Status);
            entity.Property(x => x.CreatedAtUtc)
                .IsRequired();
        });

        // Audit Log Configuration
        modelBuilder.Entity<AuditLog>(entity =>
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(x => x.AuditLogId);

            entity.Property(x => x.Action)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasKey(x => x.Id);

            entity.Property(x => x.EntityName)
            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.EntityId)
                .HasMaxLength(100);

            entity.Property(x => x.PerformedBy)
                .HasMaxLength(256);

            entity.Property(x => x.Details)
                .HasMaxLength(2000);
                .HasMaxLength(200);

            entity.Property(x => x.OccurredAtUtc)
            entity.Property(x => x.Message)
                .IsRequired();

            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.IsRead);
            entity.HasIndex(x => x.CreatedAt);
            entity.HasIndex(x => x.OccurredAtUtc);
        });
    }
}
