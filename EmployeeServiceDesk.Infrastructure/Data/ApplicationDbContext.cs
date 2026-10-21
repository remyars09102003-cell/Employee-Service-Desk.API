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

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<User> Users => Set<User>();

    public DbSet<PasswordResetOtp> PasswordResetOtps =>
        Set<PasswordResetOtp>();
}