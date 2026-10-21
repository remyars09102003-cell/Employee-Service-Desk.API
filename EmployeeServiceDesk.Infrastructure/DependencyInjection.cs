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
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        // Member 2 repositories and helpers
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IServiceItemRepository, ServiceItemRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<ITicketNumberGenerator, TicketNumberGenerator>();

        // Cross-module stubs: owners swap these for real implementations.
        services.AddScoped<ICurrentUserService, StubCurrentUserService>();   // Member 1
        services.AddScoped<ISlaService, StubSlaService>();                   // Member 3
        services.AddScoped<INotificationService, StubNotificationService>(); // Member 3
        services.AddScoped<IAuditService, StubAuditService>();               // Member 4
        services.AddScoped<IAssetLinkService, StubAssetLinkService>();       // Member 4

        return services;
    }
}
