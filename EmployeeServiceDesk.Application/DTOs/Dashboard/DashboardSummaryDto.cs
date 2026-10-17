
using System;
using System.Collections.Generic;

namespace EmployeeServiceDesk.Application.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        // Existing asset statistics
        public int TotalAssets { get; set; }
        public int TotalAssetTypes { get; set; }
        public int TotalAssignments { get; set; }

        public List<DashboardBreakdownDto> AssetsByStatus { get; set; }
            = new List<DashboardBreakdownDto>();

        public List<DashboardBreakdownDto> AssetsByType { get; set; }
            = new List<DashboardBreakdownDto>();

        // Additional asset reports
        public int AssetsUnderRepair { get; set; }
        public int RetiredAssets { get; set; }
        public int ExpiredWarranties { get; set; }
        public int WarrantiesExpiringSoon { get; set; }

        // Feedback statistics
        public int TotalFeedback { get; set; }
        public double AverageFeedbackRating { get; set; }

        public List<DashboardBreakdownDto> FeedbackByRating { get; set; }
            = new List<DashboardBreakdownDto>();

        public List<DashboardBreakdownDto> FeedbackByStatus { get; set; }
            = new List<DashboardBreakdownDto>();
    }
}