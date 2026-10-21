using Application.ApiResult;
using Domain.Entities;
using Domain.Filters;

namespace Application.Interfaces;

public interface IProductRepository
{
    Task<PagedResult<Product>> GetPagedProductsAsync(ProductFilter filter, CancellationToken cancellationToken);
    Task<Product?> GetProductByIdAsync(int id);
    Task<int> CreateProductAsync(Product product);
    Task<int> UpdateProductAsync(Product product);
    Task<int> DeleteProductAsync(Product product);
}