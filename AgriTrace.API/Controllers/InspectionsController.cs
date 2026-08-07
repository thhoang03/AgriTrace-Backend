using AgriTrace.API.Models;
using AgriTrace.Application.Features.Inspections.Commands;
using AgriTrace.Application.Features.Inspections.Queries;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using MediatR;
using AgriTrace.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

[ApiController]
[Authorize]
[Produces("application/json")]
public sealed class InspectionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;
    private readonly IBatchReadService _batchReadService;

    public InspectionsController(IMediator mediator, ICurrentUserService currentUser, IBatchReadService batchReadService)
    {
        _mediator = mediator;
        _currentUser = currentUser;
        _batchReadService = batchReadService;
    }

    private bool IsAdminOrInspectionOrg =>
        string.Equals(_currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase)
     || string.Equals(_currentUser.OrganizationType, "Inspection", StringComparison.OrdinalIgnoreCase);

    // ─────────────────────────────────────────────────────────────
    // GET /api/v1/inspections
    // ─────────────────────────────────────────────────────────────

    [HttpGet("api/v1/inspections")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllInspections(
        [FromQuery] Guid? batchId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (batchId.HasValue)
        {
            return await GetInspectionsByBatch(batchId.Value, page, pageSize, cancellationToken);
        }

        Guid? orgId = string.Equals(_currentUser.OrganizationType, "Inspection", StringComparison.OrdinalIgnoreCase)
            ? _currentUser.OrganizationId
            : null;

        var result = await _mediator.Send(
            new GetQualityInspectionsPagedQuery(orgId, page, pageSize),
            cancellationToken);

        var items = result.Items.Select(ToResponse).ToList();
        var response = new InspectionPagedResponse(items, result.TotalCount, result.PageNumber, result.PageSize);

        return Ok(ApiResponse.Success(response));
    }

    // ─────────────────────────────────────────────────────────────
    // POST /api/v1/batches/{batchId}/inspections
    // ─────────────────────────────────────────────────────────────

    [HttpPost("api/v1/batches/{batchId:guid}/inspections")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateInspection(
        Guid batchId,
        [FromBody] CreateInspectionRequest request,
        CancellationToken cancellationToken)
    {
        if (!IsAdminOrInspectionOrg)
            return Forbid();

        var batch = await _batchReadService.GetByIdAsync(batchId, cancellationToken);
        if (batch is null)
            return NotFound(ErrorResponse.Fail($"Batch '{batchId}' was not found."));

        var inspectorId = _currentUser.UserId;
        var orgId = string.Equals(_currentUser.OrganizationType, "Inspection", StringComparison.OrdinalIgnoreCase)
            ? _currentUser.OrganizationId
            : request.OrganizationId;

        var dto = await _mediator.Send(
            new CreateQualityInspectionCommand(
                batch.Id,
                inspectorId,
                orgId,
                request.InspectionType,
                request.InspectionDate,
                request.Notes),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetInspectionById),
            new { inspectionId = dto.Id },
            ApiResponse.Success(new { inspectionId = dto.Id }));
    }

    // ─────────────────────────────────────────────────────────────
    // POST /api/v1/batches/{batchId}/inspections/request
    // ─────────────────────────────────────────────────────────────

    [HttpPost("api/v1/batches/{batchId:guid}/inspections/request")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RequestInspection(
        Guid batchId,
        [FromBody] RequestInspectionBody request,
        CancellationToken cancellationToken)
    {
        // Any authenticated user from a farm/manufacturer can request inspection
        var dto = await _mediator.Send(
            new RequestQualityInspectionCommand(
                batchId,
                request.TargetOrganizationId,
                request.Notes),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetInspectionById),
            new { inspectionId = dto.Id },
            ApiResponse.Success(new { inspectionId = dto.Id }));
    }

    // ─────────────────────────────────────────────────────────────
    // GET /api/v1/batches/{batchId}/inspections
    // ─────────────────────────────────────────────────────────────

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
        var response = new InspectionPagedResponse(items, result.TotalCount, result.PageNumber, result.PageSize);

        return Ok(ApiResponse.Success(response));
    }

    // ─────────────────────────────────────────────────────────────
    // GET /api/v1/inspections/{inspectionId}
    // ─────────────────────────────────────────────────────────────

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
            return NotFound(ErrorResponse.Fail($"Inspection '{inspectionId}' was not found."));
        }

        return Ok(ApiResponse.Success(ToResponse(dto)));
    }

    // ─────────────────────────────────────────────────────────────
    // PUT /api/v1/inspections/{inspectionId} — Conclude
    // ─────────────────────────────────────────────────────────────

    [HttpPut("api/v1/inspections/{inspectionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConcludeInspection(
        Guid inspectionId,
        [FromBody] ConcludeInspectionRequest request,
        CancellationToken cancellationToken)
    {
        if (!IsAdminOrInspectionOrg)
            return Forbid();

        await _mediator.Send(
            new ConcludeInspectionCommand(inspectionId, request.OverallResult, request.Notes),
            cancellationToken);

        return Ok(ApiResponse.Success("Cập nhật kiểm định thành công"));
    }

    // ─────────────────────────────────────────────────────────────
    // Lab Test Endpoints
    // ─────────────────────────────────────────────────────────────

    [HttpPost("api/v1/inspections/{inspectionId:guid}/lab-tests")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddLabTest(
        Guid inspectionId,
        [FromBody] AddLabTestRequestBody request,
        CancellationToken cancellationToken)
    {
        if (!IsAdminOrInspectionOrg)
            return Forbid();

        var dto = await _mediator.Send(
            new AddLabTestCommand(
                inspectionId,
                request.TestName,
                request.MeasuredValue,
                request.Unit,
                request.MinStandardValue,
                request.MaxStandardValue,
                request.IsPassed,
                request.Remark),
            cancellationToken);

        return CreatedAtAction(nameof(GetInspectionById), new { inspectionId }, ApiResponse.Success(dto));
    }

    [HttpGet("api/v1/inspections/{inspectionId:guid}/lab-tests")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLabTests(
        Guid inspectionId,
        CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(
            new GetQualityInspectionByIdQuery(inspectionId),
            cancellationToken);

        if (dto is null)
            return NotFound(ErrorResponse.Fail($"Inspection '{inspectionId}' was not found."));

        return Ok(ApiResponse.Success(dto.LabTests));
    }

    [HttpDelete("api/v1/lab-tests/{labTestId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveLabTest(
        Guid labTestId,
        CancellationToken cancellationToken)
    {
        if (!IsAdminOrInspectionOrg)
            return Forbid();

        await _mediator.Send(new RemoveLabTestCommand(labTestId), cancellationToken);
        return Ok(ApiResponse.Success("Lab test removed."));
    }

    // ─────────────────────────────────────────────────────────────
    // Mapping
    // ─────────────────────────────────────────────────────────────

    private static InspectionResponse ToResponse(
        AgriTrace.Application.Contracts.QualityInspectionDto dto)
    {
        return new InspectionResponse
        {
            InspectionId = dto.Id,
            BatchId = dto.BatchId,
            BatchCode = dto.BatchCode,
            OrganizationId = dto.OrganizationId,
            InspectorId = dto.InspectorId,
            InspectorName = dto.InspectorName,
            InspectionType = dto.InspectionType,
            Status = dto.Status,
            OverallResult = dto.OverallResult,
            InspectionDate = dto.InspectionDate,
            Notes = dto.Notes,
            CreatedAt = dto.CreatedAt,
            LabTests = dto.LabTests.Select(t => new LabTestResponse
            {
                Id = t.Id,
                TestName = t.TestName,
                MeasuredValue = t.MeasuredValue,
                Unit = t.Unit,
                MinStandardValue = t.MinStandardValue,
                MaxStandardValue = t.MaxStandardValue,
                IsPassed = t.IsPassed,
                Remark = t.Remark,
                CreatedAt = t.CreatedAt
            }).ToList()
        };
    }
}

public record AddLabTestRequestBody(
    string TestName,
    string? MeasuredValue,
    string? Unit,
    string? MinStandardValue,
    string? MaxStandardValue,
    bool IsPassed,
    string? Remark);

public record ConcludeInspectionRequest(
    string OverallResult,
    string? Notes);

public record RequestInspectionBody(
    Guid TargetOrganizationId,
    string? Notes);
