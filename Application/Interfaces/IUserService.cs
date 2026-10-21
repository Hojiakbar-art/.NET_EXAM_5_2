using Application.ApiResult;
using Application.DTOs;
using Application.DTOs.User;
using Domain.Filters;

namespace Application.Interfaces;

public interface IUserService
{
    Task<Result<PagedResult<UserDto>>> GetPagedAsync(UserFilter filter, CancellationToken ct);
    Task<Result<UserDto>> GetByIdAsync(int id);
    Task<Result<UserDto>> UpdateAsync(int id,  UpdateUserDto user);
    Task<Result<bool>> DeleteAsync(int id);
}
