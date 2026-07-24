using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using AgriTrace.API.Mapping;
using AgriTrace.API.Models;
using AgriTrace.Application.Features.Batches.Commands;
using AgriTrace.Application.Features.Batches.Queries;


namespace AgriTrace.API.Controllers;


/// <summary>
/// Quản lý Batch (lô nông sản) và các thao tác liên quan.
/// </summary>
[ApiController]
[Route("api/v1/batches")]
[Authorize]
public sealed class BatchesController : ControllerBase
{

    private readonly ISender _sender;


    public BatchesController(
        ISender sender)
    {
        _sender = sender;
    }



    /// <summary>
    /// Danh sách Batch
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        Guid? productId,
        Guid? organizationId,
        string? search,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {

        var result = await _sender.Send(
            new GetBatchesQuery(
                productId,
                organizationId,
                search,
                page,
                pageSize),
            cancellationToken);

        var paged = new BatchPagedResponse(
            result.Items.Select(x => x.ToListItemResponse()),
            result.TotalCount,
            result.PageNumber,
            result.PageSize);

        return Ok(ApiResponse.Success(paged));

    }



    /// <summary>
    /// Chi tiết Batch
    /// </summary>
    [HttpGet("{batchId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid batchId,
        CancellationToken cancellationToken)
    {

        var result = await _sender.Send(
            new GetBatchByIdQuery(batchId),
            cancellationToken);

        return Ok(ApiResponse.Success(result.ToResponse()));

    }



    /// <summary>
    /// Tạo Batch mới
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateBatchRequest request,
        CancellationToken cancellationToken)
    {

        var created = await _sender.Send(
            request.ToCommand(),
            cancellationToken);

        var createdDto = await _sender.Send(
            new GetBatchByIdQuery(created.Id),
            cancellationToken);

        var createdData = new BatchCreatedData
        {
            BatchId = createdDto.Id,
            BatchCode = createdDto.BatchCode,
            QrCodeUrl = createdDto.QrCodeUrl
        };

        return CreatedAtAction(
            nameof(GetById),
            new { batchId = created.Id },
            ApiResponse.Success(createdData));

    }



    /// <summary>
    /// Cập nhật Batch
    /// </summary>
    [HttpPut("{batchId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid batchId,
        [FromBody] UpdateBatchRequest request,
        CancellationToken cancellationToken)
    {

        var batch = await _sender.Send(
            request.ToCommand(batchId),
            cancellationToken);

        return Ok(ApiResponse.Success(batch.ToResponse(), "Batch updated successfully."));

    }



    /// <summary>
    /// Thay đổi trạng thái Batch
    /// </summary>
    [HttpPatch("{batchId:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        Guid batchId,
        [FromBody] BatchStatusRequest request,
        CancellationToken cancellationToken)
    {

        await _sender.Send(
            new UpdateBatchStatusCommand(batchId, request.Status),
            cancellationToken);

        return Ok(ApiResponse.Success("Batch status updated."));

    }



    /// <summary>
    /// Lấy QR Code của Batch
    /// </summary>
    [HttpGet("{batchId:guid}/qr-code")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQrCode(
        Guid batchId,
        CancellationToken cancellationToken)
    {

        var batch = await _sender.Send(
            new GetBatchByIdQuery(batchId),
            cancellationToken);

        var qrCodeData = new QrCodeData
        {
            BatchId = batch.Id,
            BatchCode = batch.BatchCode,
            QrCodeUrl = batch.QrCodeUrl,
            PublicTraceUrl = $"/api/v1/public/trace/{batchId}"
        };

        return Ok(ApiResponse.Success(qrCodeData));

    }



    /// <summary>
    /// Danh sách ảnh Batch
    /// </summary>
    [HttpGet("{batchId:guid}/images")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public IActionResult GetImages(
        Guid batchId)
    {
        // TODO: implement image listing (Phase 8+). Stub returns an empty list.
        return Ok(ApiResponse.Success(new { items = Array.Empty<object>() }));
    }



    /// <summary>
    /// Upload ảnh Batch
    /// </summary>
    [HttpPost("{batchId:guid}/images")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status501NotImplemented)]
    public IActionResult UploadImage(
        Guid batchId,
        [FromForm] ImageUploadRequest request)
    {
        // TODO: implement image storage (cloud storage) — deferred to a later phase.
        return StatusCode(
            501,
            ErrorResponse.Fail("Image upload not implemented yet."));
    }



    /// <summary>
    /// Xóa ảnh Batch
    /// </summary>
    [HttpDelete("images/{imageId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status501NotImplemented)]
    public IActionResult DeleteImage(
        Guid imageId)
    {
        // TODO: implement image deletion — deferred to a later phase.
        return StatusCode(
            501,
            ErrorResponse.Fail("Image deletion not implemented yet."));
    }



    // Not in swagger.yaml — internal-only endpoint, suppressed from OpenAPI docs (Phase 12 decision: keep suppressed).
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpDelete("{batchId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid batchId,
        CancellationToken cancellationToken)
    {

        await _sender.Send(
            new DeleteBatchCommand(batchId),
            cancellationToken);

        return NoContent();

    }

}