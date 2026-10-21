using Application.ApiResult;
using Application.Identity;
using Application.Interfaces;
using Domain.Filters;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<PagedResult<User>> GetPagedAsync(UserFilter filter, CancellationToken cancellation)
    {
        var query = context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();

            query = query.Where(u =>
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search) ||
                u.Email!.ToLower().Contains(search));
        }

        return await query.ToPagedListAsync(filter.Page, filter.PageSize, cancellation);
    }

    public async Task<User?> GetByIdAsync(int id)
        => await context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> GetByEmailAsync(string email)
        => context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<int> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user.Id;
    }

    public async Task<int> UpdateAsync(User user)
    {
        context.Users.Update(user);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(User user)
    {
        context.Users.Remove(user);
        return await context.SaveChangesAsync();
    }
}