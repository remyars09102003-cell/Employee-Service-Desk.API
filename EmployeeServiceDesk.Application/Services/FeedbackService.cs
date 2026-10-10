using EmployeeServiceDesk.Application.DTOs.Feedback;
using EmployeeServiceDesk.Application.ServiceInterface;
using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.RepositoryInterface;

namespace EmployeeServiceDesk.Application.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IAuditRepository _auditRepository;

    public FeedbackService(
        IFeedbackRepository feedbackRepository,
        IAuditRepository auditRepository)
    {
        _feedbackRepository = feedbackRepository;
        _auditRepository = auditRepository;
    }

    public async Task<IReadOnlyList<FeedbackDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var feedbackList =
            await _feedbackRepository.GetAllAsync(cancellationToken);

        return feedbackList.Select(MapToDto).ToList();
    }

    public async Task<FeedbackDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var feedback = await _feedbackRepository.GetByIdAsync(
            id, cancellationToken);

        return feedback is null ? null : MapToDto(feedback);
    }

    public async Task<FeedbackDto> CreateAsync(
        CreateFeedbackDto dto,
        CancellationToken cancellationToken = default)
    {
        var feedback = new Feedback
        {
            EmployeeName = dto.EmployeeName.Trim(),
            EmployeeEmail = dto.EmployeeEmail?.Trim(),
            Category = dto.Category.Trim(),
            Rating = dto.Rating,
            Comments = dto.Comments.Trim(),
            Status = "Submitted",
            CreatedAtUtc = DateTime.UtcNow
        };

        await _feedbackRepository.AddAsync(
            feedback, cancellationToken);

        // Save first so FeedbackId is generated.
        await _feedbackRepository.SaveChangesAsync(cancellationToken);

        var auditLog = new AuditLog
        {
            Action = "FeedbackCreated",
            EntityName = "Feedback",
            EntityId = feedback.FeedbackId.ToString(),
            PerformedBy = feedback.EmployeeEmail ?? feedback.EmployeeName,
            Details = "Feedback submitted.",
            OccurredAtUtc = DateTime.UtcNow
        };

        await _auditRepository.AddAsync(auditLog, cancellationToken);
        await _auditRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(feedback);
    }

    public async Task<FeedbackDto?> UpdateStatusAsync(
        int id,
        UpdateFeedbackStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        var feedback = await _feedbackRepository.GetByIdAsync(
            id, cancellationToken);

        if (feedback is null)
        {
            return null;
        }

        var oldStatus = feedback.Status;

        feedback.Status = dto.Status;
        feedback.UpdatedAtUtc = DateTime.UtcNow;

        await _feedbackRepository.SaveChangesAsync(cancellationToken);

        var auditLog = new AuditLog
        {
            Action = "FeedbackStatusUpdated",
            EntityName = "Feedback",
            EntityId = feedback.FeedbackId.ToString(),
            PerformedBy = "System",
            Details = $"Status changed from {oldStatus} to {feedback.Status}.",
            OccurredAtUtc = DateTime.UtcNow
        };

        await _auditRepository.AddAsync(auditLog, cancellationToken);
        await _auditRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(feedback);
    }

    private static FeedbackDto MapToDto(Feedback feedback)
    {
        return new FeedbackDto
        {
            FeedbackId = feedback.FeedbackId,
            EmployeeName = feedback.EmployeeName,
            EmployeeEmail = feedback.EmployeeEmail,
            Category = feedback.Category,
            Rating = feedback.Rating,
            Comments = feedback.Comments,
            Status = feedback.Status,
            CreatedAtUtc = feedback.CreatedAtUtc,
            UpdatedAtUtc = feedback.UpdatedAtUtc
        };
    }
}