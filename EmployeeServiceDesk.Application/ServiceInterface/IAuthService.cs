using EmployeeServiceDesk.Application.DTOs.Auth;

namespace EmployeeServiceDesk.Application.ServiceInterface
{
    public interface IAuthService
    {
        Task<LoginResponseDto> RegisterAsync(
            RegisterRequestDto dto);

        Task<LoginResponseDto> LoginAsync(
            LoginDto dto);

        Task<LoginResponseDto> RefreshTokenAsync(
            string refreshToken);

        Task<bool> LogoutAsync(
            string refreshToken);

        // Forgot Password methods
        Task ForgotPasswordAsync(
            ForgotPasswordRequestDto dto);

        Task<bool> VerifyOtpAsync(
            VerifyOtpDto dto);

        Task<bool> ResetPasswordAsync(
            ResetPasswordDto dto);
    }
}