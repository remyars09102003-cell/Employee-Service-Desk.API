
using EmployeeServiceDesk.Application.DTOs.Asset;
using EmployeeServiceDesk.Application.ServiceInterface;
using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.Enums;
using EmployeeServiceDesk.Domain.RepositoryInterface;

namespace EmployeeServiceDesk.Application.Services;

public class AssetService : IAssetService
{
    private readonly IAssetRepository _assetRepository;
    private readonly IAuditRepository _auditRepository;

    public AssetService(
        IAssetRepository assetRepository,
        IAuditRepository auditRepository)
    {
        _assetRepository = assetRepository;
        _auditRepository = auditRepository;
    }

    public async Task<List<AssetDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var assets = await _assetRepository.GetAllAsync(cancellationToken);

        return assets.Select(MapToDto).ToList();
    }

    public async Task<AssetDto?> GetByIdAsync(
        int assetId,
        CancellationToken cancellationToken = default)
    {
        var asset = await _assetRepository.GetByIdAsync(
            assetId, cancellationToken);

        return asset == null ? null : MapToDto(asset);
    }

    // CREATE ASSET
    public async Task<AssetDto> CreateAsync(
        CreateAssetDto dto,
        CancellationToken cancellationToken = default)
    {
        var existing = await _assetRepository.GetByAssetTagAsync(
            dto.AssetTag, cancellationToken);

        if (existing != null)
        {
            throw new InvalidOperationException(
                $"Asset tag '{dto.AssetTag}' already exists.");
        }

        var asset = new Asset
        {
            AssetTag = dto.AssetTag.Trim(),
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            SerialNumber = dto.SerialNumber?.Trim(),
            AssetTypeId = dto.AssetTypeId,
            PurchaseDate = dto.PurchaseDate,
            WarrantyExpiryDate = dto.WarrantyExpiryDate,
            Status = AssetStatus.Available,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _assetRepository.AddAsync(asset, cancellationToken);
        await _assetRepository.SaveChangesAsync(cancellationToken);

        await WriteAuditAsync(
            "AssetCreated",
            asset,
            $"Asset '{asset.Name}' was created.",
            cancellationToken);

        return MapToDto(asset);
    }

    // UPDATE ASSET
    public async Task<AssetDto?> UpdateAsync(
        int assetId,
        UpdateAssetDto dto,
        CancellationToken cancellationToken = default)
    {
        var asset = await _assetRepository.GetByIdAsync(
            assetId, cancellationToken);

        if (asset == null)
            return null;

        asset.Name = dto.Name.Trim();
        asset.Description = dto.Description?.Trim();
        asset.SerialNumber = dto.SerialNumber?.Trim();
        asset.AssetTypeId = dto.AssetTypeId;
        asset.PurchaseDate = dto.PurchaseDate;
        asset.WarrantyExpiryDate = dto.WarrantyExpiryDate;
        asset.IsActive = dto.IsActive;
        asset.UpdatedAtUtc = DateTime.UtcNow;

        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _assetRepository.SaveChangesAsync(cancellationToken);

        await WriteAuditAsync(
            "AssetUpdated",
            asset,
            $"Asset '{asset.Name}' was updated.",
            cancellationToken);

        return MapToDto(asset);
    }

    // SOFT DELETE / RETIRE ASSET
    public async Task<bool> DeleteAsync(
        int assetId,
        CancellationToken cancellationToken = default)
    {
        var asset = await _assetRepository.GetByIdAsync(
            assetId, cancellationToken);

        if (asset == null)
            return false;

        asset.IsActive = false;
        asset.Status = AssetStatus.Retired;
        asset.UpdatedAtUtc = DateTime.UtcNow;

        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _assetRepository.SaveChangesAsync(cancellationToken);

        await WriteAuditAsync(
            "AssetRetired",
            asset,
            $"Asset '{asset.Name}' was retired.",
            cancellationToken);

        return true;
    }

    // ASSIGN ASSET
    public async Task<bool> AssignAsync(
        int assetId,
        AssignAssetDto dto,
        CancellationToken cancellationToken = default)
    {
        var asset = await _assetRepository.GetByIdAsync(
            assetId, cancellationToken);

        if (asset == null)
            return false;

        if (asset.Status == AssetStatus.Retired)
        {
            throw new InvalidOperationException(
                "A retired asset cannot be assigned.");
        }

        var assignment = new AssetAssignment
        {
            AssetId = assetId,
            EmployeeId = dto.EmployeeId,
            AssignedByUserId = dto.AssignedByUserId,
            AssignedAtUtc = DateTime.UtcNow,
            Notes = dto.Notes?.Trim()
        };

        asset.Status = AssetStatus.InUse;
        asset.AssignedEmployeeId = dto.EmployeeId;
        asset.UpdatedAtUtc = DateTime.UtcNow;
        asset.Assignments.Add(assignment);

        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _assetRepository.SaveChangesAsync(cancellationToken);

        await WriteAuditAsync(
            "AssetAssigned",
            asset,
            $"Asset '{asset.Name}' was assigned to employee ID {dto.EmployeeId}.",
            cancellationToken,
            dto.AssignedByUserId.ToString());

        return true;
    }

    // RETURN ASSET
    public async Task<bool> ReturnAsync(
        int assetId,
        CancellationToken cancellationToken = default)
    {
        var asset = await _assetRepository.GetByIdAsync(
            assetId, cancellationToken);

        if (asset == null)
            return false;

        var activeAssignment = asset.Assignments
            .Where(x => x.ReturnedAtUtc == null)
            .OrderByDescending(x => x.AssignedAtUtc)
            .FirstOrDefault();

        if (activeAssignment != null)
        {
            activeAssignment.ReturnedAtUtc = DateTime.UtcNow;
        }

        asset.AssignedEmployeeId = null;
        asset.Status = AssetStatus.Available;
        asset.UpdatedAtUtc = DateTime.UtcNow;

        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _assetRepository.SaveChangesAsync(cancellationToken);

        await WriteAuditAsync(
            "AssetReturned",
            asset,
            $"Asset '{asset.Name}' was returned and marked available.",
            cancellationToken);

        return true;
    }

    // COMMON AUDIT LOGGING METHOD
    private async Task WriteAuditAsync(
        string action,
        Asset asset,
        string details,
        CancellationToken cancellationToken,
        string? performedBy = null)
    {
        var auditLog = new AuditLog
        {
            Action = action,
            EntityName = "Asset",
            EntityId = asset.AssetId.ToString(),
            PerformedBy = performedBy ?? "System",
            Details = details,
            OccurredAtUtc = DateTime.UtcNow
        };

        await _auditRepository.AddAsync(auditLog, cancellationToken);
        await _auditRepository.SaveChangesAsync(cancellationToken);
    }

    private static AssetDto MapToDto(Asset asset)
    {
        return new AssetDto
        {
            AssetId = asset.AssetId,
            AssetTag = asset.AssetTag,
            Name = asset.Name,
            Description = asset.Description,
            SerialNumber = asset.SerialNumber,
            AssetTypeId = asset.AssetTypeId,
            AssetTypeName = asset.AssetType?.Name,
            PurchaseDate = asset.PurchaseDate,
            WarrantyExpiryDate = asset.WarrantyExpiryDate,
            Status = asset.Status.ToString(),
            AssignedEmployeeId = asset.AssignedEmployeeId,
            IsActive = asset.IsActive
        };
    }
}

