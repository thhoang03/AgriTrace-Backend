using AgriTrace.API.Models;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Categories.Commands;
using AgriTrace.Application.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

[ApiController]
[Route("api/v1/categories")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public CategoriesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] CategoryQuery query, CancellationToken cancellationToken)
    {
        var search = query.Search?.Trim();
        var result = await _sender.Send(new GetCategoriesPagedQuery(
            search,
            query.PageNumber,
            query.PageSize), cancellationToken);

        var response = new CategoryPagedResponse(
            result.Items.Select(ToResponse),
            result.TotalCount,
            result.PageNumber,
            result.PageSize);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var category = await _sender.Send(new GetCategoryByIdQuery(id), cancellationToken);
        return Ok(ToResponse(category));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        var created = await _sender.Send(new CreateCategoryCommand(
            request.Name,
            request.Description), cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToResponse(created));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        var updated = await _sender.Send(new UpdateCategoryCommand(
            id,
            request.Name,
            request.Description), cancellationToken);

        return Ok(ToResponse(updated));
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateCategoryStatusRequest request, CancellationToken cancellationToken)
    {
        var updated = await _sender.Send(new UpdateCategoryStatusCommand(
            id,
            request.IsActive), cancellationToken);

        return Ok(ToResponse(updated));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteCategoryCommand(id), cancellationToken);
        return Ok(ApiResponse.Success(null));
    }

    private static CategoryResponse ToResponse(CategoryDto category)
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
