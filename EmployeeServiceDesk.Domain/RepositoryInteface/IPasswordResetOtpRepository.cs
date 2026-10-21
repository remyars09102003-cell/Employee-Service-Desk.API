using EmployeeServiceDesk.Domain.Entities;

namespace EmployeeServiceDesk.Domain.RepositoryInterface
{
    public interface IPasswordResetOtpRepository
    {
        Task<PasswordResetOtp?> GetLatestActiveAsync(int userId);

        Task<bool> HasRecentOtpAsync(
            int userId,
            DateTime sinceUtc);

        Task InvalidateUnusedOtpsAsync(int userId);

        Task AddAsync(PasswordResetOtp otp);

        Task SaveChangesAsync();
    }
}