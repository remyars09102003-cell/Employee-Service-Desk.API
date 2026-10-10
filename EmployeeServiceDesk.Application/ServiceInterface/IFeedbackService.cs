using EmployeeServiceDesk.Application.DTOs.Feedback;

namespace EmployeeServiceDesk.Application.ServiceInterface;

public interface IFeedbackService
{
    Task<IReadOnlyList<FeedbackDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<FeedbackDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<FeedbackDto> CreateAsync(
        CreateFeedbackDto dto,
        CancellationToken cancellationToken = default);

    Task<FeedbackDto?> UpdateStatusAsync(
        int id,
        UpdateFeedbackStatusDto dto,
        CancellationToken cancellationToken = default);
}