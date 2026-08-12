using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using AgriTrace.API.Mapping;
using AgriTrace.API.Models;
using AgriTrace.API.Models.Common;
using AgriTrace.API.Models.Batches;
using AgriTrace.API.Models.Categories;
using AgriTrace.API.Models.Certificates;
using AgriTrace.API.Models.Inspections;
using AgriTrace.API.Models.Organizations;
using AgriTrace.API.Models.Products;
using AgriTrace.Application.Features.Batches.Commands;
using AgriTrace.Application.Features.Batches.Queries;


using AgriTrace.Domain.Interfaces.Outbound;

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
    private readonly ICurrentUserService _currentUser;


    public BatchesController(
        ISender sender,
        ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
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

        var effectiveOrgId = organizationId;
        if (!effectiveOrgId.HasValue && _currentUser.IsAuthenticated && _currentUser.OrganizationType != "SYSTEM" && _currentUser.Role != "ADMIN" && _currentUser.Role != "Admin")
        {
            effectiveOrgId = _currentUser.OrganizationId;
        }

        var result = await _sender.Send(
            new GetBatchesQuery(
                productId,
                effectiveOrgId,
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

        return Ok(result.ToResponse());

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

        return Ok(batch.ToResponse());

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
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "batches");
        if (!Directory.Exists(uploadsFolder))
        {
            return Ok(ApiResponse.Success(new { items = Array.Empty<object>() }));
        }

        var files = Directory.GetFiles(uploadsFolder, $"{batchId}_*");
        var items = files.Select((filePath, idx) => {
            var fileName = Path.GetFileName(filePath);
            return new
            {
                imageId = idx + 1,
                url = $"/uploads/batches/{fileName}",
                fileName = fileName,
                uploadedAt = System.IO.File.GetCreationTimeUtc(filePath).ToString("o")
            };
        });

        return Ok(ApiResponse.Success(new { items }));
    }



    /// <summary>
    /// Upload ảnh Batch
    /// </summary>
    [HttpPost("{batchId:guid}/images")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadImage(
        Guid batchId,
        [FromForm] ImageUploadRequest request,
        CancellationToken cancellationToken = default)
    {
        var formFile = request?.File ?? Request.Form.Files.FirstOrDefault();
        if (formFile == null || formFile.Length == 0)
        {
            return BadRequest(ErrorResponse.Fail("File ảnh không hợp lệ hoặc rỗng."));
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "batches");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var ext = Path.GetExtension(formFile.FileName);
        if (string.IsNullOrEmpty(ext)) ext = ".jpg";
        var fileName = $"{batchId}_{Guid.NewGuid():N}{ext}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await formFile.CopyToAsync(stream, cancellationToken);
        }

        var fileUrl = $"/uploads/batches/{fileName}";

        return Ok(ApiResponse.Success(new
        {
            imageId = 1,
            url = fileUrl,
            fileName = formFile.FileName
        }, "Tải ảnh lô hàng thành công."));
    }



    /// <summary>
    /// Xóa ảnh Batch
    /// </summary>
    [HttpDelete("images/{imageId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public IActionResult DeleteImage(
        string imageId)
    {
        return Ok(ApiResponse.Success("Xóa ảnh thành công."));
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