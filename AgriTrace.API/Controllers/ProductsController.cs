using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using AgriTrace.API.Mapping;
using AgriTrace.API.Models;
using AgriTrace.Application.Features.Products.Commands;
using AgriTrace.Application.Features.Products.Queries;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Quản lý Product (sản phẩm nông nghiệp).
/// </summary>
[ApiController]
[Route("api/v1/products")]
[Authorize]
public sealed class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Danh sách Product
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetAll(
        Guid? organizationId,
        Guid? categoryId,
        string? search,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetProductsQuery(
                organizationId,
                categoryId,
                search,
                page,
                pageSize),
            cancellationToken);

        var paged = new ProductPagedResponse(
            result.Items.Select(x => x.ToListItemResponse()),
            result.TotalCount,
            result.PageNumber,
            result.PageSize);

        return Ok(ApiResponse.Success(paged));
    }

    /// <summary>
    /// Chi tiết Product
    /// </summary>
    [HttpGet("{productId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetById(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetProductByIdQuery(productId),
            cancellationToken);

        if (result is null)
        {
            return NotFound(
                ErrorResponse.Fail(
                    "Product not found."));
        }

        return Ok(
            ApiResponse.Success(
                result.ToResponse()));
    }

    /// <summary>
    /// Tạo Product mới
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> Create(
        ProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _sender.Send(
            request.ToCommand(),
            cancellationToken);

        return Created(
            $"/api/v1/products/{product.Id}",
            ApiResponse.Success(
                new ProductCreatedData
                {
                    Id = product.Id,
                    Name = product.Name
                }));
    }

    /// <summary>
    /// Cập nhật Product
    /// </summary>
    [HttpPut("{productId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Update(
        Guid productId,
        ProductRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            request.ToCommand(productId),
            cancellationToken);

        return Ok(
            ApiResponse.Success(
                "Product updated successfully."));
    }

    /// <summary>
    /// Xóa Product
    /// </summary>
    [HttpDelete("{productId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(
        Guid productId,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new DeleteProductCommand(productId),
            cancellationToken);

        return Ok(ApiResponse.Success("Product deleted successfully."));
    }

    /// <summary>
    /// Thay đổi trạng thái Product
    /// </summary>
    [HttpPatch("{productId:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> UpdateStatus(
        Guid productId,
        [FromBody] ProductStatusRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new UpdateProductStatusCommand(productId, request.Status),
            cancellationToken);

        return Ok(ApiResponse.Success("Product status updated."));
    }

    /// <summary>
    /// Danh sách ảnh sản phẩm
    /// </summary>
    [HttpGet("{productId:guid}/images")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse> GetImages(
        Guid productId)
    {
        // TODO: implement image listing (Phase 8+). Stub returns an empty list.
        return Ok(ApiResponse.Success(new { items = Array.Empty<object>() }));
    }

    /// <summary>
    /// Upload ảnh sản phẩm
    /// </summary>
    [HttpPost("{productId:guid}/images")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status501NotImplemented)]
    public ActionResult<ApiResponse> UploadImage(
        Guid productId,
        [FromForm] ImageUploadRequest request)
    {
        // TODO: implement image storage (cloud storage) — deferred to a later phase.
        return StatusCode(
            501,
            ErrorResponse.Fail("Image upload not implemented yet."));
    }

    /// <summary>
    /// Xóa ảnh sản phẩm
    /// </summary>
    [HttpDelete("images/{imageId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status501NotImplemented)]
    public ActionResult<ApiResponse> DeleteImage(
        Guid imageId)
    {
        // TODO: implement image deletion — deferred to a later phase.
        return StatusCode(
            501,
            ErrorResponse.Fail("Image deletion not implemented yet."));
    }
}