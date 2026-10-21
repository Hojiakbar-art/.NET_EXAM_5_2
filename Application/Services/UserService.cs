using Application.ApiResult;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Identity;
using Domain.Filters;

public class UserService(IUserRepository user) : IUserService
{
    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var exist = await user.GetByIdAsync(id);

        if (exist == null)
            return Result<bool>.Fail("Пользователь не найден", ErrorType.NotFound);

        await user.DeleteAsync(exist);

        return Result<bool>.Success(true);
    }

    public async Task<Result<UserDto>> GetByIdAsync(int id)
    {
        var res = await user.GetByIdAsync(id);

        if (res == null)
        {
            return Result<UserDto>.Fail("This user not found", ErrorType.NotFound);
        }


        var dto = new UserDto
        {
            Id = res.Id,
            FirstName = res.FirstName,
            LastName = res.LastName,
            BirthDate = res.BirthDate,
            Email = res.Email!
        };

        return Result<UserDto>.Success(dto);
    }

    public async Task<Result<PagedResult<UserDto>>> GetPagedAsync(UserFilter filter, CancellationToken ct)
    {
        if (filter.Page < 1) filter.Page = 1;
        if (filter.PageSize > 100) filter.PageSize = 100;
        if (filter.PageSize < 10) filter.PageSize = 10;

        var pagedUser = await user.GetPagedAsync(filter, ct);

        var item = pagedUser.Items.Select(us => new UserDto
        {
            Id = us.Id,
            FirstName = us.FirstName,
            LastName = us.LastName,
            BirthDate = us.BirthDate,
            Email = us.Email!
        }
        ).ToList();

        return Result<PagedResult<UserDto>>.Success(
            new PagedResult<UserDto>(item, pagedUser.Page, pagedUser.PageSize, pagedUser.TotalCount)
        );
    }

    public async Task<Result<UserDto>> UpdateAsync(int id, UpdateUserDto upUser)
    {
        if (upUser.FirstName == null) return Result<UserDto>.Fail("FirstName must be not null", ErrorType.Validation);
        if (upUser.LastName == null) return Result<UserDto>.Fail("LastName must be not null", ErrorType.Validation);

        var exist = await user.GetByIdAsync(id);

        if (exist == null) return Result<UserDto>.Fail("This user not found", ErrorType.NotFound);

        exist.FirstName = upUser.FirstName.Trim();
        exist.LastName = upUser.LastName.Trim();
        exist.BirthDate = upUser.BirthDate;


        await user.UpdateAsync(exist);

        var dto = new UserDto
        {
            Id = exist.Id,
            FirstName = exist.FirstName,
            LastName = exist.LastName,
            BirthDate = exist.BirthDate,
            Email = exist.Email!
        };

        return Result<UserDto>.Success(dto);
    }

}