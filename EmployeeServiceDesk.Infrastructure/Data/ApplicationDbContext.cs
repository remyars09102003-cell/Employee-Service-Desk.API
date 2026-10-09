using EmployeeServiceDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace EmployeeServiceDesk.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Asset> Assets => Set<Asset>();

    public DbSet<AssetType> AssetTypes => Set<AssetType>();

    public DbSet<AssetAssignment> AssetAssignments => Set<AssetAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
    }
}