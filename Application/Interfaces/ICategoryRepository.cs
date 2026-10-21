using Application.ApiResult;
using Domain.Entities;
using Domain.Filters;

namespace Application.Interfaces;

public interface ICategoryRepository
{
    Task<PagedResult<Category>> GetPagedCategoryAsync(CategoryFilter filter, CancellationToken cancellationToken);
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<int> CreateCategoryAsync(Category category);
    Task<int> UpdateCategoryAsync(Category category);
    Task<int> DeleteCategoryAsync(Category category);
    Task<PagedResult<Category>> GetPagedCategoryWithProductsAsync(CategoryFilter filter, CancellationToken cancellationToken);
}