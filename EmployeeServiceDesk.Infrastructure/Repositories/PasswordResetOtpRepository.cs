using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.RepositoryInterface;
using EmployeeServiceDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeServiceDesk.Infrastructure.Repositories
{
    public class PasswordResetOtpRepository
        : IPasswordResetOtpRepository
    {
        private readonly ApplicationDbContext _context;

        public PasswordResetOtpRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PasswordResetOtp?> GetLatestActiveAsync(
            int userId)
        {
            return await _context.PasswordResetOtps
                .Where(o => o.UserId == userId && !o.IsUsed)
                .OrderByDescending(o => o.CreatedAtUtc)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasRecentOtpAsync(
            int userId,
            DateTime sinceUtc)
        {
            return await _context.PasswordResetOtps
                .AnyAsync(o =>
                    o.UserId == userId &&
                    o.CreatedAtUtc >= sinceUtc);
        }

        public async Task InvalidateUnusedOtpsAsync(int userId)
        {
            var records = await _context.PasswordResetOtps
                .Where(o => o.UserId == userId && !o.IsUsed)
                .ToListAsync();

            foreach (var record in records)
            {
                record.IsUsed = true;
            }
        }

        public async Task AddAsync(PasswordResetOtp otp)
        {
            await _context.PasswordResetOtps.AddAsync(otp);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}