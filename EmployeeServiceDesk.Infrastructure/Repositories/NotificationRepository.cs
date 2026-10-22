using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.Interfaces;
using EmployeeServiceDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmployeeServiceDesk.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NotificationRepository> _logger;

    public NotificationRepository(ApplicationDbContext context, ILogger<NotificationRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Notification>> GetByUserIdAsync(int userId)
    {
        try
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting notifications for user {UserId}",
                userId);

            throw;
        }
    }

    public async Task<Notification?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting notification {Id}",
                id);

            throw;
        }
    }

    public async Task<Notification> AddAsync(Notification notification)
    {
        try
        {
            await _context.Notifications
                .AddAsync(notification);

            await _context.SaveChangesAsync();

            return notification;
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
            var notification =
                await _context.Notifications
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (notification == null)
                return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error marking notification {Id} as read",
                id);

            throw;
        }
    }
}