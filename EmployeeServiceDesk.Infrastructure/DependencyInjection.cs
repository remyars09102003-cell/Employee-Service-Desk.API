using EmployeeServiceDesk.Application.Mapper;
using EmployeeServiceDesk.Application.ServiceInterface;
using EmployeeServiceDesk.Domain.RepositoryInterface;
using EmployeeServiceDesk.Infrastructure.Data;
using EmployeeServiceDesk.Infrastructure.Repositories;
using EmployeeServiceDesk.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceDesk.Application.Abstractions;
using ServiceDesk.Application.Abstractions.Persistence;
using ServiceDesk.Infrastructure.Persistence;
using ServiceDesk.Infrastructure.Persistence.Repositories;
using ServiceDesk.Infrastructure.Stubs;

namespace ServiceDesk.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
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

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();

        // Member 2 repositories and helpers
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IServiceItemRepository, ServiceItemRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<ITicketNumberGenerator, TicketNumberGenerator>();
        // Password reset OTP repository
        services.AddScoped<
            IPasswordResetOtpRepository,
            PasswordResetOtpRepository>();

        // Cross-module stubs: owners swap these for real implementations.
        services.AddScoped<ICurrentUserService, StubCurrentUserService>();   // Member 1
        services.AddScoped<ISlaService, StubSlaService>();                   // Member 3
        services.AddScoped<INotificationService, StubNotificationService>(); // Member 3
        services.AddScoped<IAuditService, StubAuditService>();               // Member 4
        services.AddScoped<IAssetLinkService, StubAssetLinkService>();       // Member 4
        // Infrastructure services
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}
