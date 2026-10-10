using EmployeeServiceDesk.Application.DTOs.Dashboard;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeServiceDesk.Application.ServiceInterface
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync(
        CancellationToken cancellationToken = default);
    }
}
