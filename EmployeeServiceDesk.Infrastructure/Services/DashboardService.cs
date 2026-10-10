
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmployeeServiceDesk.Application.DTOs;
using EmployeeServiceDesk.Application.DTOs.Dashboard;
using EmployeeServiceDesk.Application.ServiceInterface;
using EmployeeServiceDesk.Domain.Enums;
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
            var today = DateTime.UtcNow.Date;
            var thirtyDaysFromNow = today.AddDays(30);

            // Existing asset statistics
            var totalAssets = await _context.Assets
                .CountAsync(cancellationToken);

            var totalAssetTypes = await _context.AssetTypes
                .CountAsync(cancellationToken);

            var totalAssignments = await _context.AssetAssignments
                .CountAsync(cancellationToken);

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

            // Additional asset statistics
            var assetsUnderRepair = await _context.Assets
                .CountAsync(
                    asset => asset.Status == AssetStatus.Repair,
                    cancellationToken);

            var retiredAssets = await _context.Assets
                .CountAsync(
                    asset => asset.Status == AssetStatus.Retired,
                    cancellationToken);

            var expiredWarranties = await _context.Assets
                .CountAsync(
                    asset => asset.WarrantyExpiryDate < today,
                    cancellationToken);

            var warrantiesExpiringSoon = await _context.Assets
                .CountAsync(
                    asset => asset.WarrantyExpiryDate >= today
                          && asset.WarrantyExpiryDate <= thirtyDaysFromNow,
                    cancellationToken);

            // Feedback statistics
            var feedbackQuery = _context.FeedbackEntries.AsNoTracking();

            var totalFeedback = await feedbackQuery
                .CountAsync(cancellationToken);

            var averageFeedbackRating = totalFeedback == 0
                ? 0
                : await feedbackQuery
                    .AverageAsync(
                        feedback => (double)feedback.Rating,
                        cancellationToken);

            var feedbackRatings = await feedbackQuery
                .Select(feedback => feedback.Rating)
                .ToListAsync(cancellationToken);

            var feedbackByRating = feedbackRatings
                .GroupBy(rating => rating)
                .Select(group => new DashboardBreakdownDto
                {
                    Label = group.Key + "/5",
                    Count = group.Count()
                })
                .OrderBy(item => item.Label)
                .ToList();

            var feedbackStatuses = await feedbackQuery
                .Select(feedback => feedback.Status)
                .ToListAsync(cancellationToken);

            var feedbackByStatus = feedbackStatuses
                .GroupBy(status => status)
                .Select(group => new DashboardBreakdownDto
                {
                    Label = group.Key,
                    Count = group.Count()
                })
                .OrderBy(item => item.Label)
                .ToList();

            // Return the complete dashboard summary
            return new DashboardSummaryDto
            {
                TotalAssets = totalAssets,
                TotalAssetTypes = totalAssetTypes,
                TotalAssignments = totalAssignments,
                AssetsByStatus = assetsByStatus,
                AssetsByType = assetsByType,

                AssetsUnderRepair = assetsUnderRepair,
                RetiredAssets = retiredAssets,
                ExpiredWarranties = expiredWarranties,
                WarrantiesExpiringSoon = warrantiesExpiringSoon,

                TotalFeedback = totalFeedback,
                AverageFeedbackRating = Math.Round(
                    averageFeedbackRating, 2),
                FeedbackByRating = feedbackByRating,
                FeedbackByStatus = feedbackByStatus
            };
        }
    }
}