using Application.DTOs.Categories; 
using Application.Interfaces;
using Domain.Enums;
using Domain.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/categories")]
public class CategoriesController(ICategoryService categoryService) : ApiController
{
    [HttpGet("paged")]
    public async Task<IActionResult> GetAll([FromQuery] CategoryFilter filter, CancellationToken ct)
        => ToResponse(await categoryService.GetPagedCategoryAsync(filter, ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => ToResponse(await categoryService.GetCategoryByIdAsync(id));

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Create(CreateCategoryDto dto)
        => ToResponse(await categoryService.CreateCategoryAsync(dto));

    [HttpPut]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Update(UpdateCategoryDto dto)
        => ToResponse(await categoryService.UpdateCategoryAsync(dto));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Delete(int id)
        => ToResponse(await categoryService.DeleteCategoryAsync(id));
}