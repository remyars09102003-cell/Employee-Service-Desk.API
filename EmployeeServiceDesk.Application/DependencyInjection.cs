using EmployeeServiceDesk.Application.Abstractions.Tickets.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ServiceDesk.Application.Catalog.Services;
using ServiceDesk.Application.Catalog.Validators;

namespace ServiceDesk.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Member 2: catalog + tickets
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IServiceItemService, ServiceItemService>();
        services.AddScoped<ITicketService, TicketService>();

        // Registers every validator in this assembly (all members' validators live here).
        services.AddValidatorsFromAssemblyContaining<CreateCategoryRequestValidator>();
        return services;
    }
}
