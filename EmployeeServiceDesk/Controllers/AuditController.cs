
using EmployeeServiceDesk.Application.ServiceInterface;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeServiceDesk.Controllers;

[ApiController]
[Route("api/audit")]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    // GET: api/audit?take=50
    [HttpGet]
    public async Task<IActionResult> GetRecent(
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        var logs = await _auditService.GetRecentAsync(
            take, cancellationToken);

        return Ok(logs);
    }
}

