using EmployeeServiceDesk.Application.DTOs.SLA;
using EmployeeServiceDesk.Application.Interfaces;
using EmployeeServiceDesk.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeServiceDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SLAController : ControllerBase
{
    private readonly ISLAService _service;
    private readonly ILogger<SLAController> _logger;

    public SLAController(ISLAService service,ILogger<SLAController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _service.GetAllAsync();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching SLA rules");

            return StatusCode(
                500,
                new
                {
                    message = "Unable to fetch SLA rules."
                });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound(new
                {
                    message = "SLA not found."
                });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error fetching SLA {Id}",
                id);

            return StatusCode(
                500,
                new
                {
                    message = "Unable to fetch SLA."
                });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSlaDto dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating SLA");

            return StatusCode(
                500,
                new
                {
                    message = "Unable to create SLA."
                });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id,[FromBody] UpdateSlaDto dto)
    {
        try
        {
            var result = await _service.UpdateAsync(id, dto);

            if (result == null)
                return NotFound(new
                {
                    message = "SLA not found."
                });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error updating SLA {Id}",
                id);

            return StatusCode(
                500,
                new
                {
                    message = "Unable to update SLA."
                });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound(new
                {
                    message = "SLA not found."
                });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error deleting SLA {Id}",
                id);

            return StatusCode(
                500,
                new
                {
                    message = "Unable to delete SLA."
                });
        }
    }

    [HttpGet("calculate/{priority}")]
    public async Task<IActionResult> CalculateDeadline(Priority priority)
    {
        try
        {
            var result = await _service.CalculateDeadlineAsync(priority);

            return Ok(new
            {
                priority,
                responseDeadline = result.ResponseDeadline,
                resolutionDeadline = result.ResolutionDeadline
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error calculating SLA deadline");

            return StatusCode(
                500,
                new
                {
                    message = "Unable to calculate SLA deadline."
                });
        }
    }
}