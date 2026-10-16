using EmployeeServiceDesk.Domain.Entities;

namespace EmployeeServiceDesk.Domain.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetByUserIdAsync(
        int userId);

    Task<Notification?> GetByIdAsync(int id);

    Task<Notification> AddAsync(
        Notification notification);

    Task<bool> MarkAsReadAsync(int id);
}