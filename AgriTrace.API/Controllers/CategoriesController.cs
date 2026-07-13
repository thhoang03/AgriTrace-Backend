using System;
using System.Threading.Tasks;
using AgriTrace.API.Models;
using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

[ApiController]
[Route("api/v1/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CategoryQuery query, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetPagedAsync(
            query.Search,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        var response = new
        {
            result.TotalCount,
            result.PageNumber,
            result.PageSize,
            result.TotalPages,
            Items = result.Items.Select(ToResponse)
        };

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException($"Category {id} not found.");
        }

        return Ok(ToResponse(category));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        if (await _categoryService.GetByNameAsync(request.Name, cancellationToken) != null)
        {
            throw new ConflictException("Category name already exists.");
        }

        var category = new Category(request.Name, request.Description);
        var created = await _categoryService.CreateAsync(category, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToResponse(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        var existing = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            throw new NotFoundException($"Category {id} not found.");
        }

        var duplicate = await _categoryService.GetByNameAsync(request.Name, cancellationToken);
        if (duplicate != null && duplicate.Id != id)
        {
            throw new ConflictException("Category name already exists.");
        }

        existing.UpdateInformation(request.Name, request.Description);
        await _categoryService.UpdateAsync(existing, cancellationToken);

        return Ok(ToResponse(existing));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateCategoryStatusRequest request, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException($"Category {id} not found.");
        }

        category.ChangeStatus(request.IsActive);
        await _categoryService.UpdateAsync(category, cancellationToken);

        return Ok(ToResponse(category));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException($"Category {id} not found.");
        }

        if (await _categoryService.HasProductsAsync(id, cancellationToken))
        {
            throw new ConflictException("Cannot delete category because it is being used by products.");
        }

        await _categoryService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }

    private static CategoryResponse ToResponse(Category category)
    {
        return new CategoryResponse
        {
            CategoryId = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}
