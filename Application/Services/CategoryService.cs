using Domain.Filters;
using Domain.Entities;
using Application.ApiResult;
using Application.DTOs.User;
using Application.Interfaces;
using Application.DTOs.Categories;
using Application.Identity;

namespace Application.Services;

public class CategoryService(ICategoryRepository repository) : ICategoryService
{
    public async Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryDto createCategory)
{

    if (string.IsNullOrWhiteSpace(createCategory.Name))
    {
        return Result<CategoryDto>.Fail("Category name cannot be empty", ErrorType.Validation);
    }
    var newCategory = new Category
    {
        Name = createCategory.Name.Trim(),
        Description = createCategory.Description?.Trim() 
    };

    await repository.CreateCategoryAsync(newCategory);
    var dto = new CategoryDto
    {
        Id = newCategory.Id, 
        Name = newCategory.Name,
        Description = newCategory.Description
    };

    return Result<CategoryDto>.Success(dto);
}

    public async Task<Result<bool>> DeleteCategoryAsync(int id)
    {
        var exist = await repository.GetCategoryByIdAsync(id);

        if(exist == null)
            return Result<bool>.Fail("Category not found", ErrorType.NotFound);
        
        await repository.DeleteCategoryAsync(exist);
        return Result<bool>.Success(true);
    }

    public async Task<Result<CategoryDto?>> GetCategoryByIdAsync(int id)
    {
        var res = await repository.GetCategoryByIdAsync(id);

        if(res == null)
            return Result<CategoryDto?>.Fail("This category not found", ErrorType.NotFound);
        
        var dto = new CategoryDto
        {
            Id = res.Id,
            Name = res.Name,
            Description = res.Description
        };
        return Result<CategoryDto?>.Success(dto);
    }

    public async Task<Result<PagedResult<CategoryDto>>> GetPagedCategoryAsync(CategoryFilter filter, CancellationToken cancellationToken)
    {
        if (filter.Page < 1) filter.Page = 1;
        if (filter.PageSize > 100) filter.PageSize = 100;
        if (filter.PageSize < 10) filter.PageSize = 10;

        var pagedCategory = await repository.GetPagedCategoryAsync(filter, cancellationToken);

        var item = pagedCategory.Items.Select(c=> new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }).ToList();

        return Result<PagedResult<CategoryDto>>.Success(
            new PagedResult<CategoryDto>(item, pagedCategory.Page, pagedCategory.PageSize, pagedCategory.TotalCount)
        );
    }
    public async Task<Result<CategoryDto>> UpdateCategoryAsync(UpdateCategoryDto category)
    {
        if(category.Name == null) return Result<CategoryDto>.Fail("This category must be not null", ErrorType.Validation);

        var exist = await repository.GetCategoryByIdAsync(category.Id);

        if(exist == null) return Result<CategoryDto>.Fail("This category not found", ErrorType.NotFound);

        exist.Name = category.Name.Trim();
        await repository.UpdateCategoryAsync(exist);

        var c = new CategoryDto
        {
            Id = exist.Id,
            Name = exist.Name,
            Description = exist.Name
        };
        return Result<CategoryDto>.Success(c);
    }
}