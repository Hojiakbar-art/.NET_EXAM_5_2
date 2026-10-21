using Domain.Filters;
using Domain.Entities;
using Application.ApiResult;
using Application.DTOs.Products;
using Application.Interfaces;

namespace Application.Services;

public class ProductService(IProductRepository repository) : IProductService
{
    public async Task<Result<ProductDto>> CreateProductAsync(CreateProductDto createProduct)
    {
        if (string.IsNullOrWhiteSpace(createProduct.Name))
        {
            return Result<ProductDto>.Fail("Product name cannot be empty", ErrorType.Validation);
        }

        var newProduct = new Product
        {
            Name = createProduct.Name.Trim(),
            Description = createProduct.Description?.Trim(),
            Price = createProduct.Price,
            Stock = createProduct.Stock,
            CategoryId = createProduct.CategoryId
        };

        await repository.CreateProductAsync(newProduct);

        var dto = new ProductDto
        {
            Id = newProduct.Id,
            Name = newProduct.Name,
            Description = newProduct.Description,
            Price = newProduct.Price,
            Stock = newProduct.Stock,
            CategoryId = newProduct.CategoryId,
            IsActive = newProduct.IsActive
        };

        return Result<ProductDto>.Success(dto);
    }

    public async Task<Result<bool>> DeleteProductAsync(int id)
    {
        var exist = await repository.GetProductByIdAsync(id);

        if (exist == null)
            return Result<bool>.Fail("Product not found", ErrorType.NotFound);

        await repository.DeleteProductAsync(exist);
        return Result<bool>.Success(true);
    }

    public async Task<Result<ProductDto?>> GetProductByIdAsync(int id)
    {
        var res = await repository.GetProductByIdAsync(id);

        if (res == null)
            return Result<ProductDto?>.Fail("This product not found", ErrorType.NotFound);

        var dto = new ProductDto
        {
            Id = res.Id,
            Name = res.Name,
            Description = res.Description,
            Price = res.Price,
            Stock = res.Stock,
            CategoryId = res.CategoryId,
            IsActive = res.IsActive,
            CategoryName = res.Category?.Name ?? string.Empty
        };
        return Result<ProductDto?>.Success(dto);
    }

    public async Task<Result<PagedResult<ProductDto>>> GetPagedProductsAsync(ProductFilter filter, CancellationToken cancellationToken)
    {
        if (filter.Page < 1) filter.Page = 1;
        if (filter.PageSize > 100) filter.PageSize = 100;
        if (filter.PageSize < 10) filter.PageSize = 10;

        var pagedProducts = await repository.GetPagedProductsAsync(filter, cancellationToken);

        var item = pagedProducts.Items.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            CategoryId = p.CategoryId,
            IsActive = p.IsActive,
            CategoryName = p.Category?.Name ?? string.Empty
        }).ToList();

        return Result<PagedResult<ProductDto>>.Success(
            new PagedResult<ProductDto>(item, filter.Page, filter.PageSize, pagedProducts.TotalCount)
        );
    }

    public async Task<Result<ProductDto>> UpdateProductAsync(UpdateProductDto product)
    {
        if (product.Name == null) return Result<ProductDto>.Fail("This product must be not null", ErrorType.Validation);

        var exist = await repository.GetProductByIdAsync(product.Id);

        if (exist == null) return Result<ProductDto>.Fail("This product not found", ErrorType.NotFound);

        exist.Name = product.Name.Trim();
        exist.Description = product.Description?.Trim();
        exist.Price = product.Price;
        exist.Stock = product.Stock;
        exist.IsActive = product.IsActive;
        exist.CategoryId = product.CategoryId;

        await repository.UpdateProductAsync(exist);

        var p = new ProductDto
        {
            Id = exist.Id,
            Name = exist.Name,
            Description = exist.Description,
            Price = exist.Price,
            Stock = exist.Stock,
            CategoryId = exist.CategoryId,
            IsActive = exist.IsActive
        };
        return Result<ProductDto>.Success(p);
    }
}