using EmployeeServiceDesk.Application.DTOs.Auth;
using EmployeeServiceDesk.Application.ServiceInterface;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeServiceDesk.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequestDto dto)
        {
            try
            {
                LoginResponseDto response =
                    await _authService.RegisterAsync(dto);

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginDto dto)
        {
            try
            {
                LoginResponseDto response =
                    await _authService.LoginAsync(dto);

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(
            [FromBody] RefreshTokenRequestDto dto)
        {
            try
            {
                LoginResponseDto response =
                    await _authService.RefreshTokenAsync(
                        dto.RefreshToken);

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
            [FromBody] RefreshTokenRequestDto dto)
        {
            bool result =
                await _authService.LogoutAsync(dto.RefreshToken);

            if (!result)
            {
                return BadRequest(new
                {
                    message = "Invalid refresh token."
                });
            }

            return Ok(new
            {
                message = "Logged out successfully."
            });
        }
    }

    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}