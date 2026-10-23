using ServiceDesk.Application.Tickets.Dtos;
using ServiceDesk.Domain.Entities;
using System.Net.Sockets;

namespace ServiceDesk.Application.Tickets;

internal static class TicketMappings
{
    public static TicketResponse ToResponse(this Ticket t) =>
        new(t.Id, t.TicketNumber, t.Title, t.Description, t.RequesterEmployeeId,
            t.CategoryId, t.Category.Name, t.ServiceItemId, t.ServiceItem.Name,
            t.Priority, t.Status,
            t.DueResponseAtUtc, t.DueResolutionAtUtc, t.ResolvedAtUtc, t.ClosedAtUtc,
            t.AssetId, t.CreatedAtUtc, t.UpdatedAtUtc);

    public static TicketSummaryResponse ToSummary(this Ticket t) =>
        new(t.Id, t.TicketNumber, t.Title, t.RequesterEmployeeId,
            t.Category.Name, t.ServiceItem.Name, t.Priority, t.Status,
            t.DueResolutionAtUtc, t.CreatedAtUtc);

    public static TicketHistoryResponse ToResponse(this TicketHistory h) =>
        new(h.Id, h.Action, h.FieldName, h.OldValue, h.NewValue, h.ChangedByUserId, h.ChangedAtUtc);
}
