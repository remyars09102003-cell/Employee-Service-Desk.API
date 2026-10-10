using EmployeeServiceDesk.Domain.Entities;

namespace EmployeeServiceDesk.Domain.RepositoryInterface;

public interface IFeedbackRepository
{
    Task<List<Feedback>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Feedback?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Feedback feedback,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}