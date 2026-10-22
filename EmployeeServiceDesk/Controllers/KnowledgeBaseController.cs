using EmployeeServiceDesk.Application.DTOs.Knowledge;
using EmployeeServiceDesk.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeServiceDesk.API.Controllers;

[ApiController]
[Route("api/knowledge")]
public class KnowledgeBaseController : ControllerBase
{
    private readonly IKnowledgeArticleService _service;
    private readonly ILogger<KnowledgeBaseController> _logger;

    public KnowledgeBaseController(IKnowledgeArticleService service,ILogger<KnowledgeBaseController> logger)
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
            _logger.LogError(
                ex,
                "Error getting knowledge articles");

            return StatusCode(500, new
            {
                message = "Unable to get knowledge articles."
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
                    message = "Article not found."
                });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting article {Id}",
                id);

            return StatusCode(500, new
            {
                message = "Unable to get article."
            });
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string keyword)
    {
        try
        {
            var result = await _service.SearchAsync(keyword);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error searching knowledge base");

            return StatusCode(500, new
            {
                message = "Unable to search knowledge base."
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateKnowledgeArticleDto dto)
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
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating article");

            return StatusCode(500, new
            {
                message = "Unable to create article."
            });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id,UpdateKnowledgeArticleDto dto)
    {
        try
        {
            var result = await _service.UpdateAsync(id, dto);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error updating article {Id}",
                id);

            return StatusCode(500, new
            {
                message = "Unable to update article."
            });
        }
    }

    [HttpPost("{id:int}/publish")]
    public async Task<IActionResult> Publish(int id)
    {
        try
        {
            var result = await _service.PublishAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error publishing article {Id}",
                id);

            return StatusCode(500, new
            {
                message = "Unable to publish article."
            });
        }
    }

    [HttpPost("{id:int}/archive")]
    public async Task<IActionResult> Archive(int id)
    {
        try
        {
            var result =
                await _service.ArchiveAsync(id);

            if (!result)
                return NotFound();

            return Ok(new
            {
                message = "Article archived successfully."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error archiving article {Id}",
                id);

            return StatusCode(500, new
            {
                message = "Unable to archive article."
            });
        }
    }

    [HttpPost("{id:int}/feedback")]
    public async Task<IActionResult> Feedback(int id,[FromQuery] bool helpful)
    {
        try
        {
            var result =await _service.MarkHelpfulAsync(id,helpful);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error adding article feedback");

            return StatusCode(500, new
            {
                message = "Unable to update article feedback."
            });
        }
    }
}