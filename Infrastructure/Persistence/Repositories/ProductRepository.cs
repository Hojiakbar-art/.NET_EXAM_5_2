using Application.ApiResult;
using Application.Interfaces;
using Domain.Entities;
using Domain.Filters;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<PagedResult<Product>> GetPagedProductsAsync(ProductFilter filter, CancellationToken cancellationToken)
    {
        var query = context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();

            query = query.Where(p=> 
            p.Name.ToLower().Contains(search));
        }

        if(filter.IsActive != null)
        {
            query = query.Where(x=> x.IsActive == filter.IsActive);
        }

        if(filter.CategoryId != null)
        {
            query = query.Where(x=> x.CategoryId == filter.CategoryId);
        }
        if(filter.MinPrice != null)
        {
            query = query.Where(p => p.Price >= filter.MinPrice);
        }
        if(filter.MaxPrice != null)
        {
            query = query.Where(p => p.Price <= filter.MaxPrice);
        }
        
        query = filter.SortBy?.ToLower() switch
        {
            "name" => query.OrderBy(x=> x.Name),
            "price" => query.OrderBy(x=> x.Price),
            "date" => query.OrderBy(x=> x.Id),
            _ => query.OrderBy(x=> x.Id)
        };
        return  await query.ToPagedListAsync(filter.Page, filter.PageSize, cancellationToken);
    }
    public async Task<int> CreateProductAsync(Product product)
    {
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
        return product.Id;
    }

    public async Task<int> DeleteProductAsync(Product product)
    {
        context.Products.Remove(product);
        return await context.SaveChangesAsync();
    }


    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await context.Products.FirstOrDefaultAsync(
            x=> x.Id == id);
    }

    public async Task<int> UpdateProductAsync(Product product)
    {
        context.Update(product);
        return await context.SaveChangesAsync();
    }
}