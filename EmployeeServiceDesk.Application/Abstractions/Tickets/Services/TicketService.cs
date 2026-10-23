using FluentValidation;
using ServiceDesk.Application.Abstractions;
using ServiceDesk.Application.Abstractions.Persistence;
using ServiceDesk.Application.Common;
using ServiceDesk.Application.Tickets;
using ServiceDesk.Application.Tickets.Dtos;
using ServiceDesk.Domain.Entities;
using ServiceDesk.Domain.Enums;
using ServiceDesk.Domain.Exceptions;
using System.Net.Sockets;

namespace EmployeeServiceDesk.Application.Abstractions.Tickets.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _tickets;
    private readonly IServiceItemRepository _services;
    private readonly ITicketNumberGenerator _numbers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _user;
    private readonly ISlaService _sla;
    private readonly INotificationService _notifications;
    private readonly IAssetLinkService _assets;
    private readonly IAuditService _audit;
    private readonly IValidator<CreateTicketRequest> _createValidator;
    private readonly IValidator<UpdateTicketRequest> _updateValidator;
    private readonly IValidator<TicketQuery> _queryValidator;

    public TicketService(
        ITicketRepository tickets,
        IServiceItemRepository services,
        ITicketNumberGenerator numbers,
        IUnitOfWork uow,
        ICurrentUserService user,
        ISlaService sla,
        INotificationService notifications,
        IAssetLinkService assets,
        IAuditService audit,
        IValidator<CreateTicketRequest> createValidator,
        IValidator<UpdateTicketRequest> updateValidator,
        IValidator<TicketQuery> queryValidator)
    {
        _tickets = tickets;
        _services = services;
        _numbers = numbers;
        _uow = uow;
        _user = user;
        _sla = sla;
        _notifications = notifications;
        _assets = assets;
        _audit = audit;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _queryValidator = queryValidator;
    }

    public async Task<PagedResult<TicketSummaryResponse>> GetPagedAsync(TicketQuery query, CancellationToken ct = default)
    {
        await _queryValidator.ValidateAndThrowAsync(query, ct);

        // Employees only ever see their own tickets; the requester filter is ignored for them.
        // TODO(Step 6): agents should only see tickets of their own queue(s).
        var requesterId = _user.IsStaff() ? query.RequesterEmployeeId : _user.RequireEmployeeId();

        var filter = new TicketFilter(
            string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim(),
            query.Status,
            query.Priority,
            query.CategoryId,
            query.ServiceItemId,
            requesterId,
            query.CreatedFrom.HasValue ? DateTime.SpecifyKind(query.CreatedFrom.Value.Date, DateTimeKind.Utc) : null,
            query.CreatedTo.HasValue ? DateTime.SpecifyKind(query.CreatedTo.Value.Date.AddDays(1), DateTimeKind.Utc) : null);

        var (page, pageSize) = Paging.Normalize(query.Page, query.PageSize);
        var (items, total) = await _tickets.GetPagedAsync(filter, page, pageSize, ct);

        return new PagedResult<TicketSummaryResponse>(
            items.Select(t => t.ToSummary()).ToList(), page, pageSize, total);
    }

    public async Task<TicketResponse> GetByIdAsync(int id, CancellationToken ct = default)
        => (await GetAccessibleAsync(id, ct)).ToResponse();

    public async Task<IReadOnlyList<TicketHistoryResponse>> GetHistoryAsync(int id, CancellationToken ct = default)
    {
        await GetAccessibleAsync(id, ct); // access check
        var history = await _tickets.GetHistoryAsync(id, ct);
        return history.Select(h => h.ToResponse()).ToList();
    }

    public async Task<TicketResponse> CreateAsync(CreateTicketRequest request, CancellationToken ct = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);

        var requesterId = _user.RequireEmployeeId(); // always from the token, never from the body

        var service = await _services.GetByIdAsync(request.ServiceItemId, ct)
                      ?? throw new NotFoundException(nameof(ServiceItem), request.ServiceItemId);
        if (!service.IsActive || !service.Category.IsActive)
            throw new DomainException("This service is not currently available.");

        if (request.AssetId.HasValue && !await _assets.AssetExistsAsync(request.AssetId.Value, ct))
            throw new NotFoundException("Asset", request.AssetId.Value);

        var priority = request.Priority ?? service.DefaultPriority;
        var number = await _numbers.NextAsync(ct);

        var ticket = new Ticket(number, request.Title, request.Description, requesterId, service, priority, _user.UserId);

        var deadlines = await _sla.CalculateDeadlinesAsync(priority, service.Id, ticket.CreatedAtUtc, ct);
        ticket.SetDeadlines(deadlines.ResponseDueUtc, deadlines.ResolutionDueUtc);

        if (request.AssetId.HasValue)
            ticket.LinkAsset(request.AssetId.Value, _user.UserId);

        await _tickets.AddAsync(ticket, ct);
        await _uow.SaveChangesAsync(ct);

        if (request.AssetId.HasValue)
            await _assets.LinkTicketAsync(request.AssetId.Value, ticket.Id, ct);

        await _notifications.NotifyAsync(
            NotificationType.TicketCreated, requesterId, ticket.Id, $"Ticket {ticket.TicketNumber} was created.", ct);
        await _audit.LogAsync("Create", nameof(Ticket), ticket.Id.ToString(), ticket.TicketNumber, ct);

        return ticket.ToResponse();
    }

    public async Task<TicketResponse> UpdateAsync(int id, UpdateTicketRequest request, CancellationToken ct = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);

        var ticket = await GetAccessibleAsync(id, ct);
        ticket.EnsureEditable();

        var isStaff = _user.IsStaff();
        if (!isStaff)
        {
            if (ticket.Status is not (TicketStatus.New or TicketStatus.Open or TicketStatus.WaitingForUser))
                throw new DomainException("You can only edit your ticket while it is New, Open or waiting for your reply.");
            if (request.Priority.HasValue && request.Priority.Value != ticket.Priority)
                throw new ForbiddenException("Only service desk staff can change the priority.");
        }

        var userId = _user.UserId;
        ticket.UpdateDetails(request.Title, request.Description, userId);

        var priorityChanged = isStaff
                              && request.Priority.HasValue
                              && ticket.ChangePriority(request.Priority.Value, userId);

        if (priorityChanged)
        {
            var deadlines = await _sla.CalculateDeadlinesAsync(ticket.Priority, ticket.ServiceItemId, ticket.CreatedAtUtc, ct);
            ticket.SetDeadlines(deadlines.ResponseDueUtc, deadlines.ResolutionDueUtc);
        }

        await _uow.SaveChangesAsync(ct);

        if (priorityChanged)
            await _notifications.NotifyAsync(
                NotificationType.PriorityChanged, ticket.RequesterEmployeeId, ticket.Id,
                $"Priority of {ticket.TicketNumber} changed to {ticket.Priority}.", ct);

        await _audit.LogAsync("Update", nameof(Ticket), id.ToString(), ticket.TicketNumber, ct);
        return ticket.ToResponse();
    }

    /// <summary>Staff can open any ticket; employees only their own (others look like "not found").</summary>
    private async Task<Ticket> GetAccessibleAsync(int id, CancellationToken ct)
    {
        var ticket = await _tickets.GetByIdAsync(id, ct)
                     ?? throw new NotFoundException(nameof(Ticket), id);

        if (_user.IsStaff()) return ticket;

        if (ticket.RequesterEmployeeId != _user.RequireEmployeeId())
            throw new NotFoundException(nameof(Ticket), id);

        return ticket;
    }
}
