using Application.DTOs;
using Application.DTOs.Authentication;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, IUserService userService) : ApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
        => ToResponse(await authService.RegisterAsync(dto));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
        => ToResponse(await authService.LoginAsync(dto));

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        => ToResponse(await authService.ForgotPasswordAsync(dto));

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        => ToResponse(await authService.ResetPasswordAsync(dto));
        
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
        => ToResponse(await userService.GetByIdAsync(CurrentUserId));
}