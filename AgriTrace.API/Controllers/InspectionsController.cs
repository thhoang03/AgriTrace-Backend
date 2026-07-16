using AgriTrace.API.Models;
using AgriTrace.Application.Features.Inspections.Commands;
using AgriTrace.Application.Features.Inspections.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Manages quality inspections for agricultural batches.
/// </summary>
[ApiController]
[Produces("application/json")]
public sealed class InspectionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InspectionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ─────────────────────────────────────────────────────────────
    // Nested under batches: POST /api/v1/batches/{batchId}/inspections
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Create a new quality inspection for a batch.
    /// Authorization: INSPECTOR
    /// </summary>
    [HttpPost("api/v1/batches/{batchId:guid}/inspections")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateInspection(
        Guid batchId,
        [FromBody] CreateInspectionRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(
            new CreateQualityInspectionCommand(
                batchId,
                request.InspectorId,
                request.Result,
                request.Notes),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetInspectionById),
            new { inspectionId = dto.Id },
            new { inspectionId = dto.Id });
    }

    /// <summary>
    /// Get all inspections for a specific batch.
    /// Authorization: ADMIN | MANAGER | INSPECTOR
    /// </summary>
    [HttpGet("api/v1/batches/{batchId:guid}/inspections")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInspectionsByBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var dtos = await _mediator.Send(
            new GetQualityInspectionsByBatchQuery(batchId),
            cancellationToken);

        var response = dtos.Select(ToResponse).ToList();

        return Ok(response);
    }

    // ─────────────────────────────────────────────────────────────
    // Standalone: GET /api/v1/inspections/{inspectionId}
    //             PUT /api/v1/inspections/{inspectionId}
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Get the details of a specific inspection.
    /// Authorization: ADMIN | INSPECTOR
    /// </summary>
    [HttpGet("api/v1/inspections/{inspectionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
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
                ApiResponse.Fail(
                    HttpStatusCode.NotFound,
                    $"Inspection '{inspectionId}' was not found."));
        }

        return Ok(ToResponse(dto));
    }

    /// <summary>
    /// Update the result and notes of an existing inspection.
    /// Authorization: INSPECTOR
    /// </summary>
    [HttpPut("api/v1/inspections/{inspectionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
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

        return Ok(new { message = "Cập nhật kiểm định thành công" });
    }

    // ─────────────────────────────────────────────────────────────
    // Lookup: GET /api/v1/inspection-results
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Get all available inspection result codes (Pending, PASS, FAIL).
    /// Authorization: Authenticated
    /// </summary>
    [HttpGet("api/v1/inspection-results")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInspectionResults(
        CancellationToken cancellationToken)
    {
        var results = await _mediator.Send(
            new GetInspectionResultsQuery(),
            cancellationToken);

        return Ok(results);
    }

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
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
}
