using System.Security.Cryptography;
using System.Text;

using EmployeeServiceDesk.Application.DTOs.Auth;
using EmployeeServiceDesk.Application.ServiceInterface;
using EmployeeServiceDesk.Domain.Entities;
using EmployeeServiceDesk.Domain.Enums;
using EmployeeServiceDesk.Domain.RepositoryInterface;

namespace EmployeeServiceDesk.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;
        private readonly IPasswordResetOtpRepository _otpRepository;

        public AuthService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IJwtService jwtService,
            IPasswordResetOtpRepository otpRepository)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _otpRepository = otpRepository;
        }

        // REGISTER
        public async Task<LoginResponseDto> RegisterAsync(
            RegisterRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ArgumentException(
                    "Username, email, and password are required.");
            }

            if (dto.Password.Length < 8)
            {
                throw new ArgumentException(
                    "Password must contain at least 8 characters.");
            }

            string email = dto.Email.Trim().ToLowerInvariant();

            var existingUser =
                await _userRepository.GetByEmailAsync(email);

            if (existingUser != null)
            {
                throw new InvalidOperationException(
                    "An account with this email already exists.");
            }

            var user = new User
            {
                UserName = dto.UserName.Trim(),
                Email = email,
                PasswordHash =
                    _passwordService.HashPassword(dto.Password),

                // Public registration cannot assign privileged roles.
                Role = UserRole.Employee,

                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            string accessToken =
                _jwtService.GenerateAccessToken(user);

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryUtc =
                DateTime.UtcNow.AddDays(7);

            await _userRepository.SaveChangesAsync();

            return CreateResponse(user, accessToken);
        }

        // LOGIN
        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrEmpty(dto.Password))
            {
                throw new ArgumentException(
                    "Email and password are required.");
            }

            string email = dto.Email.Trim().ToLowerInvariant();

            var user =
                await _userRepository.GetByEmailAsync(email);

            if (user == null ||
                !user.IsActive ||
                !_passwordService.VerifyPassword(
                    user.PasswordHash,
                    dto.Password))
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            string accessToken =
                _jwtService.GenerateAccessToken(user);

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryUtc =
                DateTime.UtcNow.AddDays(7);

            await _userRepository.SaveChangesAsync();

            return CreateResponse(user, accessToken);
        }

        // REFRESH TOKEN
        public async Task<LoginResponseDto> RefreshTokenAsync(
            string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentException(
                    "Refresh token is required.");
            }

            var user =
                await _userRepository.GetByRefreshTokenAsync(
                    refreshToken);

            if (user == null ||
                !user.IsActive ||
                user.RefreshTokenExpiryUtc == null ||
                user.RefreshTokenExpiryUtc <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException(
                    "Refresh token is invalid or expired.");
            }

            string accessToken =
                _jwtService.GenerateAccessToken(user);

            // Rotate the refresh token.
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryUtc =
                DateTime.UtcNow.AddDays(7);

            await _userRepository.SaveChangesAsync();

            return CreateResponse(user, accessToken);
        }

        // LOGOUT
        public async Task<bool> LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return false;
            }

            var user =
                await _userRepository.GetByRefreshTokenAsync(
                    refreshToken);

            if (user == null)
            {
                return false;
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryUtc = null;

            await _userRepository.SaveChangesAsync();

            return true;
        }

        // FORGOT PASSWORD
        public async Task ForgotPasswordAsync(
            ForgotPasswordRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException("Email is required.");
            }

            string email = dto.Email.Trim().ToLowerInvariant();

            var user =
                await _userRepository.GetByEmailAsync(email);

            // Avoid revealing whether an account exists.
            if (user == null || !user.IsActive)
            {
                return;
            }

            DateTime now = DateTime.UtcNow;

            bool recentlyRequested =
                await _otpRepository.HasRecentOtpAsync(
                    user.Id,
                    now.AddMinutes(-1));

            if (recentlyRequested)
            {
                throw new InvalidOperationException(
                    "Please wait one minute before requesting another OTP.");
            }

            await _otpRepository.InvalidateUnusedOtpsAsync(user.Id);

            string otp = GenerateOtp();

            var otpRecord = new PasswordResetOtp
            {
                UserId = user.Id,
                OtpHash = HashOtp(otp),
                CreatedAtUtc = now,
                ExpiresAtUtc = now.AddMinutes(5),
                FailedAttempts = 0,
                IsUsed = false
            };

            await _otpRepository.AddAsync(otpRecord);
            await _otpRepository.SaveChangesAsync();

            // EMAIL SENDING IS NOT IMPLEMENTED YET.
            // The OTP is stored as a hash, so the original OTP
            // cannot be retrieved from the database.
            // Add an email service and send 'otp' before returning.
        }

        // VERIFY OTP
        public async Task<bool> VerifyOtpAsync(VerifyOtpDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Otp))
            {
                return false;
            }

            string email = dto.Email.Trim().ToLowerInvariant();

            var user =
                await _userRepository.GetByEmailAsync(email);

            if (user == null || !user.IsActive)
            {
                return false;
            }

            var otpRecord =
                await _otpRepository.GetLatestActiveAsync(user.Id);

            if (otpRecord == null ||
                otpRecord.IsUsed ||
                otpRecord.ExpiresAtUtc <= DateTime.UtcNow ||
                otpRecord.FailedAttempts >= 5)
            {
                return false;
            }

            if (!OtpMatches(dto.Otp, otpRecord.OtpHash))
            {
                otpRecord.FailedAttempts++;

                await _otpRepository.SaveChangesAsync();

                return false;
            }

            return true;
        }

        // RESET PASSWORD
        public async Task<bool> ResetPasswordAsync(
            ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Otp) ||
                string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return false;
            }

            if (dto.NewPassword.Length < 8 ||
                dto.NewPassword != dto.ConfirmPassword)
            {
                return false;
            }

            string email = dto.Email.Trim().ToLowerInvariant();

            var user =
                await _userRepository.GetByEmailAsync(email);

            if (user == null || !user.IsActive)
            {
                return false;
            }

            var otpRecord =
                await _otpRepository.GetLatestActiveAsync(user.Id);

            if (otpRecord == null ||
                otpRecord.IsUsed ||
                otpRecord.ExpiresAtUtc <= DateTime.UtcNow ||
                otpRecord.FailedAttempts >= 5)
            {
                return false;
            }

            if (!OtpMatches(dto.Otp, otpRecord.OtpHash))
            {
                otpRecord.FailedAttempts++;

                await _otpRepository.SaveChangesAsync();

                return false;
            }

            user.PasswordHash =
                _passwordService.HashPassword(dto.NewPassword);

            otpRecord.IsUsed = true;

            // Revoke the stored refresh token.
            user.RefreshToken = null;
            user.RefreshTokenExpiryUtc = null;

            await _userRepository.SaveChangesAsync();
            await _otpRepository.SaveChangesAsync();

            return true;
        }

        // HELPER METHODS
        private static string GenerateRefreshToken()
        {
            byte[] randomBytes =
                RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(randomBytes);
        }

        private static string GenerateOtp()
        {
            int number =
                RandomNumberGenerator.GetInt32(100000, 1000000);

            return number.ToString("D6");
        }

        private static string HashOtp(string otp)
        {
            byte[] hash = SHA256.HashData(
                Encoding.UTF8.GetBytes(otp));

            return Convert.ToHexString(hash);
        }

        private static bool OtpMatches(
            string suppliedOtp,
            string storedHash)
        {
            if (suppliedOtp.Length != 6 ||
                !suppliedOtp.All(char.IsDigit))
            {
                return false;
            }

            byte[] suppliedHash =
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(suppliedOtp));

            byte[] storedHashBytes;

            try
            {
                storedHashBytes =
                    Convert.FromHexString(storedHash);
            }
            catch (FormatException)
            {
                return false;
            }

            return suppliedHash.Length == storedHashBytes.Length &&
                   CryptographicOperations.FixedTimeEquals(
                       suppliedHash,
                       storedHashBytes);
        }

        private static LoginResponseDto CreateResponse(
            User user,
            string accessToken)
        {
            return new LoginResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role.ToString(),
                AccessToken = accessToken,
                RefreshToken = user.RefreshToken ?? string.Empty
            };
        }
    }
}