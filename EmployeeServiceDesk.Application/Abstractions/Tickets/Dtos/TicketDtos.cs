using ServiceDesk.Domain.Enums;

namespace ServiceDesk.Application.Tickets.Dtos;

public sealed record CreateTicketRequest(
    string Title, string Description, int ServiceItemId, TicketPriority? Priority, int? AssetId);

public sealed record UpdateTicketRequest(string Title, string Description, TicketPriority? Priority);

public class TicketQuery
{
    /// <summary>Matches ticket number or title.</summary>
    public string? Search { get; set; }
    public TicketStatus? Status { get; set; }
    public TicketPriority? Priority { get; set; }
    public int? CategoryId { get; set; }
    public int? ServiceItemId { get; set; }
    /// <summary>Only honoured for service desk staff; employees always see their own tickets.</summary>
    public int? RequesterEmployeeId { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public sealed record TicketResponse(
    int Id, string TicketNumber, string Title, string Description, int RequesterEmployeeId,
    int CategoryId, string CategoryName, int ServiceItemId, string ServiceName,
    TicketPriority Priority, TicketStatus Status,
    DateTime? DueResponseAtUtc, DateTime? DueResolutionAtUtc, DateTime? ResolvedAtUtc, DateTime? ClosedAtUtc,
    int? AssetId, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);

public sealed record TicketSummaryResponse(
    int Id, string TicketNumber, string Title, int RequesterEmployeeId,
    string CategoryName, string ServiceName, TicketPriority Priority, TicketStatus Status,
    DateTime? DueResolutionAtUtc, DateTime CreatedAtUtc);

public sealed record TicketHistoryResponse(
    int Id, TicketHistoryAction Action, string? FieldName, string? OldValue, string? NewValue,
    int? ChangedByUserId, DateTime ChangedAtUtc);
