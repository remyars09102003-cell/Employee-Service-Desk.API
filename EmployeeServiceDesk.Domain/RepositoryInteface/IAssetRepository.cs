using EmployeeServiceDesk.Domain.Entities;

namespace EmployeeServiceDesk.Domain.RepositoryInterface;

public interface IAssetRepository
{
    Task<List<Asset>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Asset?> GetByIdAsync(
        int assetId,
        CancellationToken cancellationToken = default);

    Task<Asset?> GetByAssetTagAsync(
        string assetTag,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Asset asset,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Asset asset,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Asset asset,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}