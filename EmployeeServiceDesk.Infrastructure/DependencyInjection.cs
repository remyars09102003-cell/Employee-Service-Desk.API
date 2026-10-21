using EmployeeServiceDesk.Application.Mapper;
using EmployeeServiceDesk.Application.ServiceInterface;
using EmployeeServiceDesk.Domain.RepositoryInterface;
using EmployeeServiceDesk.Infrastructure.Data;
using EmployeeServiceDesk.Infrastructure.Repositories;
using EmployeeServiceDesk.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeServiceDesk.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection is missing.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // AutoMapper
        services.AddAutoMapper(
            cfg => { },
            typeof(EmployeeMappingProfile).Assembly);

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();

        // Password reset OTP repository
        services.AddScoped<
            IPasswordResetOtpRepository,
            PasswordResetOtpRepository>();

        // Infrastructure services
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}