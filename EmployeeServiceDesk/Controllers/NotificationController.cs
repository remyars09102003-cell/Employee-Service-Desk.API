using EmployeeServiceDesk.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeServiceDesk.API.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _service;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(INotificationService service,ILogger<NotificationController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        try
        {
            var result = await _service.GetByUserIdAsync(userId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting notifications");

            return StatusCode(500, new
            {
                message = "Unable to get notifications."
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNotificationRequest request)
    {
        try
        {
            var result = await _service.CreateAsync(
                    request.UserId,
                    request.Email,
                    request.Title,
                    request.Message,
                    request.Type,
                    request.TicketId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating notification");

            return StatusCode(500, new
            {
                message = "Unable to create notification."
            });
        }
    }

    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        try
        {
            var result = await _service.MarkAsReadAsync(id);

            if (!result)
                return NotFound(new
                {
                    message = "Notification not found."
                });

            return Ok(new
            {
                message =
                    "Notification marked as read."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error marking notification {Id} as read",
                id);

            return StatusCode(500, new
            {
                message =
                    "Unable to mark notification as read."
            });
        }
    }
}