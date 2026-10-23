using FluentValidation;
using ServiceDesk.Application.Abstractions;
using ServiceDesk.Application.Abstractions.Persistence;
using ServiceDesk.Application.Catalog.Dtos;
using ServiceDesk.Application.Common;
using ServiceDesk.Domain.Entities;
using ServiceDesk.Domain.Exceptions;

namespace ServiceDesk.Application.Catalog.Services;

public class ServiceItemService : IServiceItemService
{
    private readonly IServiceItemRepository _services;
    private readonly ICategoryRepository _categories;
    private readonly IUnitOfWork _uow;
    private readonly IAuditService _audit;
    private readonly IValidator<CreateServiceItemRequest> _createValidator;
    private readonly IValidator<UpdateServiceItemRequest> _updateValidator;

    public ServiceItemService(
        IServiceItemRepository services,
        ICategoryRepository categories,
        IUnitOfWork uow,
        IAuditService audit,
        IValidator<CreateServiceItemRequest> createValidator,
        IValidator<UpdateServiceItemRequest> updateValidator)
    {
        _services = services;
        _categories = categories;
        _uow = uow;
        _audit = audit;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResult<ServiceItemResponse>> GetPagedAsync(ServiceItemQuery query, CancellationToken ct = default)
    {
        var (page, pageSize) = Paging.Normalize(query.Page, query.PageSize);
        var (items, total) = await _services.GetPagedAsync(
            query.CategoryId, query.Search?.Trim(), query.IncludeInactive, page, pageSize, ct);

        return new PagedResult<ServiceItemResponse>(
            items.Select(s => s.ToResponse()).ToList(), page, pageSize, total);
    }

    public async Task<ServiceItemResponse> GetByIdAsync(int id, CancellationToken ct = default)
        => (await GetOrThrowAsync(id, ct)).ToResponse();

    public async Task<ServiceItemResponse> CreateAsync(CreateServiceItemRequest request, CancellationToken ct = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);

        var category = await GetActiveCategoryAsync(request.CategoryId, ct);
        var name = request.Name.Trim();
        if (await _services.NameExistsAsync(name, category.Id, null, ct))
            throw new ConflictException($"Service '{name}' already exists in category '{category.Name}'.");

        var service = new ServiceItem(name, request.Description, category, request.DefaultPriority);
        await _services.AddAsync(service, ct);
        await _uow.SaveChangesAsync(ct);

        await _audit.LogAsync("Create", nameof(ServiceItem), service.Id.ToString(),
            $"Name={service.Name}; Category={category.Name}", ct);
        return service.ToResponse();
    }

    public async Task<ServiceItemResponse> UpdateAsync(int id, UpdateServiceItemRequest request, CancellationToken ct = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);

        var service = await GetOrThrowAsync(id, ct);
        var category = service.CategoryId == request.CategoryId
            ? service.Category
            : await GetActiveCategoryAsync(request.CategoryId, ct);

        var name = request.Name.Trim();
        if (await _services.NameExistsAsync(name, category.Id, id, ct))
            throw new ConflictException($"Service '{name}' already exists in category '{category.Name}'.");

        service.Update(name, request.Description, category, request.DefaultPriority);
        await _uow.SaveChangesAsync(ct);

        await _audit.LogAsync("Update", nameof(ServiceItem), id.ToString(),
            $"Name={service.Name}; Category={category.Name}", ct);
        return service.ToResponse();
    }

    public async Task DeactivateAsync(int id, CancellationToken ct = default)
    {
        var service = await GetOrThrowAsync(id, ct);
        if (!service.IsActive) return;

        service.Deactivate();
        await _uow.SaveChangesAsync(ct);
        await _audit.LogAsync("Deactivate", nameof(ServiceItem), id.ToString(), null, ct);
    }

    public async Task ActivateAsync(int id, CancellationToken ct = default)
    {
        var service = await GetOrThrowAsync(id, ct);
        if (service.IsActive) return;

        if (!service.Category.IsActive)
            throw new DomainException("Cannot activate a service whose category is inactive.");

        service.Activate();
        await _uow.SaveChangesAsync(ct);
        await _audit.LogAsync("Activate", nameof(ServiceItem), id.ToString(), null, ct);
    }

    private async Task<ServiceItem> GetOrThrowAsync(int id, CancellationToken ct)
        => await _services.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(ServiceItem), id);

    private async Task<TicketCategory> GetActiveCategoryAsync(int categoryId, CancellationToken ct)
    {
        var category = await _categories.GetByIdAsync(categoryId, ct)
                       ?? throw new NotFoundException(nameof(TicketCategory), categoryId);
        if (!category.IsActive)
            throw new DomainException("Services cannot be placed in an inactive category.");
        return category;
    }
}
