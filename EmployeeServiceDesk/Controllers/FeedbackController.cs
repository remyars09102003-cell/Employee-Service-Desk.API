
using EmployeeServiceDesk.Application.DTOs.Feedback;
using EmployeeServiceDesk.Application.ServiceInterface;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeServiceDesk.Controllers;

[ApiController]
[Route("api/feedback")]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;

    public FeedbackController(IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    // GET: api/feedback
    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var feedback = await _feedbackService.GetAllAsync(
            cancellationToken);

        return Ok(feedback);
    }

    // GET: api/feedback/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var feedback = await _feedbackService.GetByIdAsync(
            id, cancellationToken);

        if (feedback is null)
            return NotFound(new { message = "Feedback not found." });

        return Ok(feedback);
    }

    // POST: api/feedback
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateFeedbackDto dto,
        CancellationToken cancellationToken)
    {
        var feedback = await _feedbackService.CreateAsync(
            dto, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = feedback.FeedbackId },
            feedback);
    }

    // PUT: api/feedback/1/status
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateFeedbackStatusDto dto,
        CancellationToken cancellationToken)
    {
        var feedback = await _feedbackService.UpdateStatusAsync(
            id, dto, cancellationToken);

        if (feedback is null)
            return NotFound(new { message = "Feedback not found." });

        return Ok(feedback);
    }
}

