using EmployeeServiceDesk.Application.DTOs.Notification;
using EmployeeServiceDesk.Application.Interfaces;
using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.Enums;
using EmployeeServiceDesk.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeServiceDesk.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly IEmailService _emailService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(INotificationRepository repository,IEmailService emailService,ILogger<NotificationService> logger)
    {
        _repository = repository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<List<NotificationDto>>GetByUserIdAsync(int userId)
    {
        try
        {
            var notifications =await _repository.GetByUserIdAsync(userId);

            return notifications
                .Select(MapToDto)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting notifications");

            throw;
        }
    }

    public async Task<NotificationDto> CreateAsync(
        int userId,
        string email,
        string title,
        string message,
        NotificationType type,
        int? ticketId)
    {
        try
        {
            var notification = new Notification
            {
                UserId = userId,
                Type = type,
                Title = title,
                Message = message,
                TicketId = ticketId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.AddAsync(notification);

            // Send email
            try
            {
                await _emailService.SendEmailAsync(
                    email,
                    title,
                    message);
            }
            catch (Exception emailEx)
            {
                // We don't want an email failure
                // to remove the in-app notification.
                _logger.LogError(
                    emailEx,
                    "Email notification failed for {Email}",
                    email);
            }

            return MapToDto(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating notification");

            throw;
        }
    }

    public async Task<bool> MarkAsReadAsync(int id)
    {
        try
        {
            return await _repository
                .MarkAsReadAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error marking notification as read");

            throw;
        }
    }

    private static NotificationDto MapToDto(Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Type = notification.Type,
            Title = notification.Title,
            Message = notification.Message,
            TicketId = notification.TicketId,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }
}