using Application.ApiResult;
using Application.Interfaces;
using Domain.Entities;
using Domain.Filters;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CategoryRepository(AppDbContext context): ICategoryRepository
{
    public async Task<PagedResult<Category>> GetPagedCategoryAsync(CategoryFilter filter, CancellationToken cancellationToken)
    {
        var query = context.Categories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();

            query = query.Where(c=> 
            c.Name.ToLower().Contains(search)); 
        }
        return await query.ToPagedListAsync(filter.Page, filter.PageSize, cancellationToken);
    }

    public async Task<int> CreateCategoryAsync(Category category)
    {
        await context.AddAsync(category);
        await context.SaveChangesAsync();
        return category.Id;
    }

    public async Task<int> DeleteCategoryAsync(Category category)
    {
        context.Remove(category);
        return await context.SaveChangesAsync();
    }

    public Task<Category?> GetCategoryByIdAsync(int id)
    {
        return context.Categories.FirstOrDefaultAsync(
            x=> x.Id == id);
    }


    public async Task<int> UpdateCategoryAsync(Category category)
    {
        context.Categories.Update(category);
        return await context.SaveChangesAsync();
    }

    public async Task<PagedResult<Category>> GetPagedCategoryWithProductsAsync(CategoryFilter filter, CancellationToken cancellationToken)
    {
        var query = context.Categories
            .AsNoTracking()
            .Include(x => x.Products)
            .AsQueryable();
            
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(search));
        }
        query = query.OrderBy(c => c.Name);
        return await query.ToPagedListAsync(filter.Page, filter.PageSize, cancellationToken);
    }
}