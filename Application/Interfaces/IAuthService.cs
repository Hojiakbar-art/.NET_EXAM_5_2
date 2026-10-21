using Application.ApiResult;
using Application.DTOs.Authentication;
using Application.DTOs.User;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<Result<UserDto>> RegisterAsync(RegisterDto dto);
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);
    Task<Result<string>> ForgotPasswordAsync(ForgotPasswordDto dto);
    Task<Result<string>> ResetPasswordAsync(ResetPasswordDto dto);
}