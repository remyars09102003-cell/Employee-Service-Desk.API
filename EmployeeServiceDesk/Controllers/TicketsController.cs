using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Application.Common;
using ServiceDesk.Application.Tickets.Dtos;
using ServiceDesk.Application.Tickets.Services;

namespace ServiceDesk.API.Controllers;

[ApiController]
[Route("api/tickets")]
[Produces("application/json")]
// TODO(Member 1): [Authorize] on the whole controller once JWT is wired up.
public class TicketsController : ControllerBase
{
    private readonly ITicketService _service;

    public TicketsController(ITicketService service) => _service = service;

    /// <summary>Search / filter tickets (status, priority, category, service, date range). Employees see only their own.</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<TicketSummaryResponse>>> GetAll(
        [FromQuery] TicketQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketResponse>> GetById(int id, CancellationToken ct)
        => Ok(await _service.GetByIdAsync(id, ct));

    /// <summary>Create a ticket. The requester is taken from the signed-in user, never from the body.</summary>
    [HttpPost]
    public async Task<ActionResult<TicketResponse>> Create(CreateTicketRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TicketResponse>> Update(int id, UpdateTicketRequest request, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, request, ct));

    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<IReadOnlyList<TicketHistoryResponse>>> GetHistory(int id, CancellationToken ct)
        => Ok(await _service.GetHistoryAsync(id, ct));
}
