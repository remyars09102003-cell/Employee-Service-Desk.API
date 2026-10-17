
using EmployeeServiceDesk.Application.ServiceInterface;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeServiceDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public ReportsController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard(
            CancellationToken cancellationToken)
        {
            var summary = await _dashboardService.GetSummaryAsync(
                cancellationToken);

            return Ok(summary);
        }
    }
}