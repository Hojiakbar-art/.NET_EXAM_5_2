using Application.ApiResult;
using Application.DTOs.Products;
using Domain.Entities;
using Domain.Filters;

namespace Application.Interfaces;

public interface IProductService
{
    Task<Result<PagedResult<ProductDto>>> GetPagedProductsAsync(ProductFilter filter, CancellationToken cancellationToken);
    Task<Result<ProductDto?>> GetProductByIdAsync(int id);
    Task<Result<ProductDto>> CreateProductAsync(CreateProductDto product);
    Task<Result<ProductDto>> UpdateProductAsync(UpdateProductDto product);
    Task<Result<bool>> DeleteProductAsync(int id);
    
}