using Application.ApiResult;
using Application.DTOs.Authentication;
using Application.Identity;
using Application.Interfaces;
using Domain.Enums;
using Application.DTOs.User;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class AuthService(UserManager<User> userManager, IJwtTokenGenerator jwt, IEmailService emailService) : IAuthService
{

    private const string ResetPurpose = "ResetPassword";

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email)) return Result<AuthResponseDto>.Fail("Email must be not empty", ErrorType.Validation);
        if (string.IsNullOrWhiteSpace(dto.Password)) return Result<AuthResponseDto>.Fail("Password must be not empty", ErrorType.Validation);

        var email = dto.Email.Trim().ToLower();
        var user = await userManager.FindByEmailAsync(email);

        if (user == null) return Result<AuthResponseDto>.Fail("Invalid email or password", ErrorType.Unauthorized);

        if (await userManager.IsLockedOutAsync(user))
            return Result<AuthResponseDto>.Fail("Account is locked. Try again later", ErrorType.Unauthorized);

        if (!await userManager.CheckPasswordAsync(user, dto.Password))
        {
            await userManager.AccessFailedAsync(user);
            return Result<AuthResponseDto>.Fail("Invalid email or password", ErrorType.Unauthorized);
        }

        await userManager.ResetAccessFailedCountAsync(user);

        var roles = await userManager.GetRolesAsync(user);
        var token = jwt.Generate(user, roles);

        var result = new AuthResponseDto
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Email = user.Email ?? string.Empty
            }
        };

        return Result<AuthResponseDto>.Success(result);
    }

    public async Task<Result<UserDto>> RegisterAsync(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName)) return Result<UserDto>.Fail("FirstName must be not null", ErrorType.Validation);
        if (string.IsNullOrWhiteSpace(dto.LastName)) return Result<UserDto>.Fail("LastName must be not null", ErrorType.Validation);
        if (string.IsNullOrWhiteSpace(dto.Email)) return Result<UserDto>.Fail("Email must be not null", ErrorType.Validation);
        if (!dto.Email.Contains("@")) return Result<UserDto>.Fail("Email must have symbol @", ErrorType.Validation);
        if (string.IsNullOrWhiteSpace(dto.Password)) return Result<UserDto>.Fail("Password must be not null", ErrorType.Validation);

        var email = dto.Email.Trim().ToLower();

        if (await userManager.FindByEmailAsync(email) != null)
            return Result<UserDto>.Fail("User with this email already exist", ErrorType.Conflict);

        var user = new User
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            BirthDate = dto.BirthDate,
            Email = email,
            UserName = email
        };

        var created = await userManager.CreateAsync(user, dto.Password);

        if (!created.Succeeded)
            return Result<UserDto>.Fail(
                string.Join("; ", created.Errors.Select(e => e.Description)),
                ErrorType.Validation);

        await userManager.AddToRoleAsync(user, nameof(UserRole.User));

        var result = new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            BirthDate = user.BirthDate,
            Email = user.Email ?? string.Empty
        };

        return Result<UserDto>.Success(result);
    }
    public async Task<Result<string>> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email)) return Result<string>.Fail("Email must be not null", ErrorType.Validation);

        var message = "If this email is registered, a code has been sent";

        var email = dto.Email.Trim().ToLower();

        var user = await userManager.FindByEmailAsync(email);

        if (user == null) return Result<string>.Success(message);

        var code = await userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultEmailProvider, ResetPurpose);

        await emailService.SendAsync(
                   user.Email!,
                   "Password reset code",
                   $"Your password reset code: {code}\nIf it was not you, just ignore this email.");

        return Result<string>.Success(message);
    }
    public async Task<Result<string>> ResetPasswordAsync(ResetPasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email)) return Result<string>.Fail("Email must be not null", ErrorType.Validation);
        if (string.IsNullOrWhiteSpace(dto.Code)) return Result<string>.Fail("Code must be not null", ErrorType.Validation);
        if (string.IsNullOrWhiteSpace(dto.NewPassword)) return Result<string>.Fail("NewPassword must be not null", ErrorType.Validation);

        var user = await userManager.FindByEmailAsync(dto.Email.Trim().ToLower());

        if (user == null) return Result<string>.Fail("Invalid or expired code", ErrorType.Validation);

        if (await userManager.IsLockedOutAsync(user))
            return Result<string>.Fail("Too many attempts. Try again later", ErrorType.Unauthorized);


        var isValid = await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultEmailProvider, ResetPurpose, dto.Code.Trim());

        if (!isValid)
        {
            await userManager.AccessFailedAsync(user);
            return Result<string>.Fail("Invalid or expired code", ErrorType.Validation);
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, dto.NewPassword);

        if (!result.Succeeded)
            return Result<string>.Fail(
                string.Join("; ", result.Errors.Select(e => e.Description)),
                ErrorType.Validation);

        await userManager.ResetAccessFailedCountAsync(user);

        return Result<string>.Success("Password has been changed");
    }
}
