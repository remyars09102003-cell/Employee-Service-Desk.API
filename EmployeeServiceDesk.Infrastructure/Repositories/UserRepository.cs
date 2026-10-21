using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.RepositoryInterface;
using EmployeeServiceDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeServiceDesk.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(
            string email,
            CancellationToken ct = default)
        {
            string normalizedEmail = email.Trim().ToLower();

            return await _context.Users
                .FirstOrDefaultAsync(
                    user => user.Email.ToLower() == normalizedEmail,
                    ct);
        }

        public async Task<User?> GetByIdAsync(
            int id,
            CancellationToken ct = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(
                    user => user.Id == id,
                    ct);
        }

        public async Task<User?> GetByRefreshTokenAsync(
            string refreshToken,
            CancellationToken ct = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(
                    user => user.RefreshToken == refreshToken,
                    ct);
        }

        public async Task AddAsync(
            User user,
            CancellationToken ct = default)
        {
            await _context.Users.AddAsync(user, ct);
        }

        public async Task SaveChangesAsync(
            CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}