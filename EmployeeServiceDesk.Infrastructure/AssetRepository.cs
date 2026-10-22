using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.RepositoryInterface;
using EmployeeServiceDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeServiceDesk.Infrastructure.Repositories;

public class AssetRepository : IAssetRepository
{
    private readonly ApplicationDbContext _context;

    public AssetRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Asset>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .Include(x => x.AssetType)
            .Include(x => x.Assignments)
            .AsNoTracking()
            .OrderBy(x => x.AssetTag)
            .ToListAsync(cancellationToken);
    }

    public async Task<Asset?> GetByIdAsync(
        int assetId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .Include(x => x.AssetType)
            .Include(x => x.Assignments)
            .FirstOrDefaultAsync(
                x => x.AssetId == assetId,
                cancellationToken);
    }

    public async Task<Asset?> GetByAssetTagAsync(
        string assetTag,
        CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .Include(x => x.AssetType)
            .FirstOrDefaultAsync(
                x => x.AssetTag == assetTag,
                cancellationToken);
    }

    public async Task AddAsync(
        Asset asset,
        CancellationToken cancellationToken = default)
    {
        await _context.Assets.AddAsync(
            asset,
            cancellationToken);
    }

    public Task UpdateAsync(
        Asset asset,
        CancellationToken cancellationToken = default)
    {
        _context.Assets.Update(asset);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        Asset asset,
        CancellationToken cancellationToken = default)
    {
        _context.Assets.Remove(asset);

        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}