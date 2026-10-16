using EmployeeServiceDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace EmployeeServiceDesk.Infrastructure.Data;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {
    }

    public DbSet<SLA> SLAs =>Set<SLA>();

    public DbSet<KnowledgeArticle>KnowledgeArticles =>Set<KnowledgeArticle>();
    public DbSet<Notification>Notifications =>Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SLA>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Priority)
                .IsRequired();

            entity.Property(x => x.ResponseTimeMinutes)
                .IsRequired();

            entity.Property(x => x.ResolutionTimeMinutes)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired();
        });

        modelBuilder.Entity<KnowledgeArticle>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(x => x.Content)
                .IsRequired();

            entity.Property(x => x.Tags)
                .HasMaxLength(500);

            entity.Property(x => x.Status)
                .IsRequired();

            entity.HasIndex(x => x.Title);
            entity.HasIndex(x => x.CategoryId);
            entity.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Message)
                .IsRequired();

            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.IsRead);
            entity.HasIndex(x => x.CreatedAt);
        });
    }
}