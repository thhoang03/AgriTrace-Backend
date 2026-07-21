using AgriTrace.API.Models;
using AgriTrace.API.Services;
using AgriTrace.Application.Features.Inspections.Commands;
using AgriTrace.Application.Features.Inspections.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Manages quality inspections for agricultural batches.
/// </summary>
[ApiController]
[Authorize]
[Produces("application/json")]
public sealed class InspectionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public InspectionsController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    // ─────────────────────────────────────────────────────────────
    // Nested under batches: POST /api/v1/batches/{batchId}/inspections
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Tạo kiểm định
    /// </summary>
    [HttpPost("api/v1/batches/{batchId:guid}/inspections")]
    [Authorize(Roles = "Inspector,Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateInspection(
        Guid batchId,
        [FromBody] CreateInspectionRequest request,
        CancellationToken cancellationToken)
    {
        var inspectorId = _currentUser.UserId;

        var dto = await _mediator.Send(
            new CreateQualityInspectionCommand(
                batchId,
                inspectorId,
                request.Result,
                request.Notes),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetInspectionById),
            new { inspectionId = dto.Id },
            new { inspectionId = dto.Id });
    }

    /// <summary>
    /// Danh sách kiểm định của Batch
    /// </summary>
    [HttpGet("api/v1/batches/{batchId:guid}/inspections")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInspectionsByBatch(
        Guid batchId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetQualityInspectionsByBatchQuery(batchId, page, pageSize),
            cancellationToken);

        var items = result.Items.Select(ToResponse).ToList();

        var response = new InspectionPagedResponse(
            items,
            result.TotalCount,
            result.PageNumber,
            result.PageSize);

        return Ok(response);
    }

    // ─────────────────────────────────────────────────────────────
    // Standalone: GET /api/v1/inspections/{inspectionId}
    //             PUT /api/v1/inspections/{inspectionId}
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Chi tiết kiểm định
    /// </summary>
    [HttpGet("api/v1/inspections/{inspectionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInspectionById(
        Guid inspectionId,
        CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(
            new GetQualityInspectionByIdQuery(inspectionId),
            cancellationToken);

        if (dto is null)
        {
            return NotFound(
                ErrorResponse.Fail(
                    $"Inspection '{inspectionId}' was not found."));
        }

        return Ok(ToResponse(dto));
    }

    /// <summary>
    /// Cập nhật kiểm định
    /// </summary>
    [HttpPut("api/v1/inspections/{inspectionId:guid}")]
    [Authorize(Roles = "Inspector,Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateInspection(
        Guid inspectionId,
        [FromBody] UpdateInspectionRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new UpdateQualityInspectionCommand(
                inspectionId,
                request.Result,
                request.Notes),
            cancellationToken);

        return Ok(ApiResponse.Success("Cập nhật kiểm định thành công"));
    }

    // ─────────────────────────────────────────────────────────────
    // Lookup: GET /api/v1/inspection-results
    // Migrated to LookupController in Phase 9.
    // ─────────────────────────────────────────────────────────────

    // ─────────────────────────────────────────────────────────────
    // Mapping helper
    // ─────────────────────────────────────────────────────────────

    private static InspectionResponse ToResponse(
        AgriTrace.Application.Contracts.QualityInspectionDto dto)
    {
        return new InspectionResponse
        {
            InspectionId = dto.Id,
            BatchId = dto.BatchId,
            BatchCode = dto.BatchCode,
            InspectorId = dto.InspectorId,
            InspectorName = dto.InspectorName,
            Status = dto.Status,
            Result = dto.Result,
            Notes = dto.Notes,
            CreatedAt = dto.CreatedAt
        };
    }
}
