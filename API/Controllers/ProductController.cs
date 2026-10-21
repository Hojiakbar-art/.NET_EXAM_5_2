using Application.DTOs.Products; 
using Application.Interfaces;
using Domain.Enums;
using Domain.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/products")]
public class ProductsController(IProductService productService) : ApiController
{
    [HttpGet("paged")]
    public async Task<IActionResult> GetAll([FromQuery] ProductFilter filter, CancellationToken ct)
        => ToResponse(await productService.GetPagedProductsAsync(filter, ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => ToResponse(await productService.GetProductByIdAsync(id));

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]

    public async Task<IActionResult> Create(CreateProductDto dto)
        => ToResponse(await productService.CreateProductAsync(dto));

    [HttpPut]
    [Authorize(Roles = nameof(UserRole.Admin))]

    public async Task<IActionResult> Update(UpdateProductDto dto)
        => ToResponse(await productService.UpdateProductAsync(dto));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    
    public async Task<IActionResult> Delete(int id)
        => ToResponse(await productService.DeleteProductAsync(id));
}