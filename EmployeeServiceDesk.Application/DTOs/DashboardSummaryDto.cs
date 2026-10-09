using System.Collections.Generic;

namespace EmployeeServiceDesk.Application.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalAssets { get; set; }
        public int TotalAssetTypes { get; set; }
        public int TotalAssignments { get; set; }

        public List<DashboardBreakdownDto> AssetsByStatus { get; set; }
            = new List<DashboardBreakdownDto>();

        public List<DashboardBreakdownDto> AssetsByType { get; set; }
            = new List<DashboardBreakdownDto>();
    }
}