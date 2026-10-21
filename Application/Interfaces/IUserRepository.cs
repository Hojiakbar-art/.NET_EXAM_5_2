using Application.ApiResult;
using Domain.Filters;
using Application.Identity;

namespace Application.Interfaces;

public interface IUserRepository
{
    public Task<PagedResult<User>> GetPagedAsync(UserFilter filter, CancellationToken cancellation);
    public Task<User> GetByIdAsync(int id);
    public Task<User?> GetByEmailAsync(string email);
    public Task<int> CreateAsync(User user);
    public Task<int> UpdateAsync(User user);
    public Task<int> DeleteAsync(User user);
}
