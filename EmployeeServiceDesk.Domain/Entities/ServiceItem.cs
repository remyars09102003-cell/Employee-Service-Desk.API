using FiftyOne.Foundation.Mobile.Detection.Entities;
using ServiceDesk.Domain.Common;
using ServiceDesk.Domain.Enums;
using ServiceDesk.Domain.Exceptions;

namespace ServiceDesk.Domain.Entities;

/// <summary>A requestable service in the catalog (the spec's "Service").</summary>
public class ServiceItem : BaseEntity
{
    private ServiceItem() { } // EF Core

    public ServiceItem(string name, string? description, TicketCategory category, TicketPriority defaultPriority)
    {
        Apply(name, description, category, defaultPriority);
    }

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public int CategoryId { get; private set; }
    public TicketCategory Category { get; private set; } = default!;
    public TicketPriority DefaultPriority { get; private set; }

    public void Update(string name, string? description, TicketCategory category, TicketPriority defaultPriority)
        => Apply(name, description, category, defaultPriority);

    private void Apply(string name, string? description, TicketCategory category, TicketPriority defaultPriority)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Service name is required.");
        if (!Enum.IsDefined(defaultPriority))
            throw new DomainException("Invalid default priority.");

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Category = category;
        CategoryId = category.Id;
        DefaultPriority = defaultPriority;
    }
}
