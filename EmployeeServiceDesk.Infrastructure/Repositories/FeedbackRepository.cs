using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.RepositoryInterface;
using EmployeeServiceDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeServiceDesk.Infrastructure.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly ApplicationDbContext _context;

    public FeedbackRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Feedback>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.FeedbackEntries
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<Feedback?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.FeedbackEntries
            .FirstOrDefaultAsync(
                x => x.FeedbackId == id,
                cancellationToken);
    }

    public async Task AddAsync(
        Feedback feedback,
        CancellationToken cancellationToken = default)
    {
        await _context.FeedbackEntries.AddAsync(
            feedback, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}