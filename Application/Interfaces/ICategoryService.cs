using Application.ApiResult;
using Application.DTOs.Categories;
using Domain.Filters;

namespace Application.Interfaces;

public interface ICategoryService
{
    Task<Result<PagedResult<CategoryDto>>> GetPagedCategoryAsync(CategoryFilter filter, CancellationToken cancellationToken);
    Task<Result<CategoryDto?>> GetCategoryByIdAsync(int id);
    Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryDto category);
    Task<Result<CategoryDto>> UpdateCategoryAsync(UpdateCategoryDto category);
    Task<Result<bool>> DeleteCategoryAsync(int id);
}