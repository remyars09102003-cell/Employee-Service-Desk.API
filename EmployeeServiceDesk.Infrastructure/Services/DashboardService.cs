
using EmployeeServiceDesk.Application.DTOs;
using EmployeeServiceDesk.Application.ServiceInterface;
using EmployeeServiceDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeServiceDesk.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync(
            CancellationToken cancellationToken = default)
        {
            var totalAssets = await _context.Assets
                .CountAsync(cancellationToken);

            var totalAssetTypes = await _context.AssetTypes
                .CountAsync(cancellationToken);

            var totalAssignments = await _context.AssetAssignments
                .CountAsync(cancellationToken);

            // Load statuses, then group them in memory.
            var statuses = await _context.Assets
                .Select(asset => asset.Status)
                .ToListAsync(cancellationToken);

            var assetsByStatus = statuses
                .GroupBy(status => status.ToString())
                .Select(group => new DashboardBreakdownDto
                {
                    Label = group.Key,
                    Count = group.Count()
                })
                .OrderBy(item => item.Label)
                .ToList();

            // Count assets by their asset type.
            var assetTypeNames = await _context.Assets
                .Where(asset => asset.AssetType != null)
                .Select(asset => asset.AssetType!.Name)
                .ToListAsync(cancellationToken);

            var assetsByType = assetTypeNames
                .GroupBy(name => name)
                .Select(group => new DashboardBreakdownDto
                {
                    Label = group.Key,
                    Count = group.Count()
                })
                .OrderBy(item => item.Label)
                .ToList();

            return new DashboardSummaryDto
            {
                TotalAssets = totalAssets,
                TotalAssetTypes = totalAssetTypes,
                TotalAssignments = totalAssignments,
                AssetsByStatus = assetsByStatus,
                AssetsByType = assetsByType
            };
        }
    }
}