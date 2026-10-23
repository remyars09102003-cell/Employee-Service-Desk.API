using ServiceDesk.Domain.Common;
using ServiceDesk.Domain.Enums;
using ServiceDesk.Domain.Exceptions;
using ServiceDesk.Domain.Workflow;

namespace ServiceDesk.Domain.Entities;

public class Ticket : BaseEntity
{
    private Ticket() { } // EF Core

    public Ticket(
        string ticketNumber, string title, string description, int requesterEmployeeId,
        ServiceItem service, TicketPriority priority, int? createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(ticketNumber)) throw new DomainException("Ticket number is required.");
        if (requesterEmployeeId <= 0) throw new DomainException("Requester is required.");
        if (!Enum.IsDefined(priority)) throw new DomainException("Invalid priority.");

        TicketNumber = ticketNumber;
        RequesterEmployeeId = requesterEmployeeId;
        ServiceItem = service;
        ServiceItemId = service.Id;
        Category = service.Category;
        CategoryId = service.CategoryId;
        Priority = priority;
        Status = TicketStatus.New;
        SetText(title, description);

        AddHistory(TicketHistoryAction.Created, null, null, ticketNumber, createdByUserId);
    }

    public string TicketNumber { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;

    // Cross-module reference: Employee is owned by Member 1, so no navigation/FK yet.
    public int RequesterEmployeeId { get; private set; }

    public int CategoryId { get; private set; }
    public TicketCategory Category { get; private set; } = default!;
    public int ServiceItemId { get; private set; }
    public ServiceItem ServiceItem { get; private set; } = default!;

    public TicketPriority Priority { get; private set; }
    public TicketStatus Status { get; private set; }

    public DateTime? DueResponseAtUtc { get; private set; }
    public DateTime? DueResolutionAtUtc { get; private set; }
    public DateTime? ResolvedAtUtc { get; private set; }
    public DateTime? ClosedAtUtc { get; private set; }
    public string? ResolutionNote { get; private set; }
    public int ReopenCount { get; private set; }

    public int? AssetId { get; private set; }

    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();
    public ICollection<TicketHistory> History { get; private set; } = new List<TicketHistory>();

    public bool IsFinal => Status is TicketStatus.Resolved or TicketStatus.Closed or TicketStatus.Cancelled;

    public void EnsureEditable()
    {
        if (IsFinal)
            throw new DomainException($"A ticket that is {Status} can no longer be edited.");
    }

    // ---------------- status workflow

    /// <summary>
    /// Moves the ticket to a new status if the workflow allows it.
    /// Reopened and Assigned have their own actions (Reopen / assignment).
    /// </summary>
    public void ChangeStatus(TicketStatus next, string? note, DateTime nowUtc, int? changedByUserId)
    {
        if (next == TicketStatus.Reopened)
            throw new DomainException("Use the reopen action to reopen a ticket.");
        if (next == TicketStatus.Assigned)
            throw new DomainException("A ticket becomes Assigned when it is assigned to an agent.");

        Transition(next, note, nowUtc, changedByUserId, TicketHistoryAction.StatusChanged);
    }

    public bool CanReopen(DateTime nowUtc, TimeSpan window)
        => Status == TicketStatus.Resolved
           && ResolvedAtUtc.HasValue
           && nowUtc - ResolvedAtUtc.Value <= window;

    public void Reopen(string reason, DateTime nowUtc, TimeSpan window, int? changedByUserId)
    {
        if (Status != TicketStatus.Resolved)
            throw new DomainException("Only a resolved ticket can be reopened.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("A reason is required to reopen a ticket.");
        if (!CanReopen(nowUtc, window))
            throw new DomainException(
                $"This ticket can no longer be reopened: the reopen window of {window.TotalDays:0} day(s) has passed.");

        Transition(TicketStatus.Reopened, reason, nowUtc, changedByUserId, TicketHistoryAction.Reopened);
        ResolvedAtUtc = null;
        ResolutionNote = null;
        ReopenCount++;
    }

    private void Transition(
        TicketStatus next, string? note, DateTime nowUtc, int? changedByUserId, TicketHistoryAction action)
    {
        if (!TicketWorkflow.IsAllowed(Status, next))
            throw new DomainException($"A ticket cannot move from {Status} to {next}.");
        if (next == TicketStatus.Resolved && string.IsNullOrWhiteSpace(note))
            throw new DomainException("A resolution note is required when resolving a ticket.");

        var old = Status;
        Status = next;

        switch (next)
        {
            case TicketStatus.Resolved:
                ResolvedAtUtc = nowUtc;
                ResolutionNote = note!.Trim();
                break;
            case TicketStatus.Closed:
                ClosedAtUtc = nowUtc;
                break;
        }

        AddHistory(action, nameof(Status), old.ToString(), next.ToString(), changedByUserId, note?.Trim());
    }

    // ---------------- details

    /// <summary>Updates title and description, recording one history row per changed field.</summary>
    public void UpdateDetails(string title, string description, int? changedByUserId)
    {
        EnsureEditable();

        var oldTitle = Title;
        var oldDescription = Description;
        SetText(title, description);

        if (oldTitle != Title)
            AddHistory(TicketHistoryAction.Updated, nameof(Title), oldTitle, Title, changedByUserId);
        if (oldDescription != Description)
            AddHistory(TicketHistoryAction.Updated, nameof(Description), oldDescription, Description, changedByUserId);
    }

    /// <returns>true when the priority actually changed.</returns>
    public bool ChangePriority(TicketPriority newPriority, int? changedByUserId)
    {
        EnsureEditable();
        if (!Enum.IsDefined(newPriority)) throw new DomainException("Invalid priority.");
        if (newPriority == Priority) return false;

        var old = Priority;
        Priority = newPriority;
        AddHistory(TicketHistoryAction.PriorityChanged, nameof(Priority), old.ToString(), newPriority.ToString(), changedByUserId);
        return true;
    }

    public void SetDeadlines(DateTime? responseDueUtc, DateTime? resolutionDueUtc)
    {
        DueResponseAtUtc = responseDueUtc;
        DueResolutionAtUtc = resolutionDueUtc;
    }

    public void LinkAsset(int assetId, int? changedByUserId)
    {
        if (assetId <= 0) throw new DomainException("Invalid asset.");
        AssetId = assetId;
        AddHistory(TicketHistoryAction.Updated, nameof(AssetId), null, assetId.ToString(), changedByUserId);
    }

    public void AddHistory(
        TicketHistoryAction action, string? fieldName, string? oldValue, string? newValue,
        int? changedByUserId, string? note = null)
        => History.Add(new TicketHistory(action, fieldName, oldValue, newValue, changedByUserId, note));

    private void SetText(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Title is required.");
        if (string.IsNullOrWhiteSpace(description)) throw new DomainException("Description is required.");
        Title = title.Trim();
        Description = description.Trim();
    }
}
