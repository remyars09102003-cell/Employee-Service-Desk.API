using ServiceDesk.Application.Common;
using ServiceDesk.Domain.Enums;

namespace ServiceDesk.Application.Catalog.Dtos;

public class ServiceItemQuery : PagedQuery
{
    public int? CategoryId { get; set; }
}

public sealed record CreateServiceItemRequest(
    string Name, string? Description, int CategoryId, TicketPriority DefaultPriority);

public sealed record UpdateServiceItemRequest(
    string Name, string? Description, int CategoryId, TicketPriority DefaultPriority);

public sealed record ServiceItemResponse(
    int Id, string Name, string? Description, int CategoryId, string CategoryName,
    TicketPriority DefaultPriority, bool IsActive, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);
