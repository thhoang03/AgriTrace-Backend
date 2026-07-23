using AgriTrace.API.Models;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Categories.Commands;
using AgriTrace.Application.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Quản lý danh mục Category (loại sản phẩm).
/// </summary>
[ApiController]
[Route("api/v1/categories")]
[Authorize]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public CategoriesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lấy danh sách Category
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] CategoryQuery query, CancellationToken cancellationToken)
    {
        var search = query.Search?.Trim();
        var result = await _sender.Send(new GetCategoriesPagedQuery(
            search,
            query.Page,
            query.PageSize), cancellationToken);

        var response = new CategoryPagedResponse(
            result.Items.Select(ToResponse),
            result.TotalCount,
            result.PageNumber,
            result.PageSize);

        return Ok(response);
    }

    /// <summary>
    /// Chi tiết Category
    /// </summary>
    [HttpGet("{categoryId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid categoryId, CancellationToken cancellationToken)
    {
        var category = await _sender.Send(new GetCategoryByIdQuery(categoryId), cancellationToken);
        return Ok(ToResponse(category));
    }

    /// <summary>
    /// Tạo Category mới
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        var created = await _sender.Send(new CreateCategoryCommand(
            request.Name,
            request.Description), cancellationToken);

        return CreatedAtAction(nameof(GetById), new { categoryId = created.Id }, ToResponse(created));
    }

    /// <summary>
    /// Cập nhật Category
    /// </summary>
    [HttpPut("{categoryId:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid categoryId, [FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        var updated = await _sender.Send(new UpdateCategoryCommand(
            categoryId,
            request.Name,
            request.Description), cancellationToken);

        return Ok(ToResponse(updated));
    }

    /// <summary>
    /// Thay đổi trạng thái Category
    /// </summary>
    [HttpPatch("{categoryId:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid categoryId, [FromBody] ActiveStatusRequest request, CancellationToken cancellationToken)
    {
        var updated = await _sender.Send(new UpdateCategoryStatusCommand(
            categoryId,
            request.IsActive), cancellationToken);

        return Ok(ToResponse(updated));
    }

    /// <summary>
    /// Xóa Category
    /// </summary>
    [HttpDelete("{categoryId:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid categoryId, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteCategoryCommand(categoryId), cancellationToken);
        return NoContent();
    }

    private static CategoryResponse ToResponse(CategoryDto category)
    {
        return new CategoryResponse
        {
            CategoryId = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive
        };
    }
}
