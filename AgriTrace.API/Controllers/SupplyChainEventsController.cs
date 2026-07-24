using AgriTrace.API.Models;
using AgriTrace.Application.Features.SupplyChainEvents.Commands;
using AgriTrace.Application.Features.SupplyChainEvents.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Records and queries supply chain events for agricultural batches.
/// Each event is chained with a SHA-256 hash to guarantee tamper-proof traceability.
/// </summary>
[ApiController]
[Produces("application/json")]
public sealed class SupplyChainEventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SupplyChainEventsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ─────────────────────────────────────────────────────────────
    // POST /api/v1/batches/{batchId}/events
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Record a new supply chain event for a batch.
    /// The hash chain (previousHash / currentHash) is computed automatically.
    /// Authorization: MANAGER | STAFF
    /// </summary>
    [HttpPost("api/v1/batches/{batchId:guid}/events")]
    [ProducesResponseType(typeof(SupplyChainEventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateEvent(
        Guid batchId,
        [FromBody] CreateSupplyChainEventRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(
            new CreateSupplyChainEventCommand(
                batchId,
                request.EventTypeId,
                request.OrganizationId,
                request.PerformedByUserId,
                request.EventData,
                request.Location),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetEventById),
            new { eventId = dto.Id },
            ToResponse(dto));
    }

    // ─────────────────────────────────────────────────────────────
    // GET /api/v1/batches/{batchId}/events  — timeline
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Get the full supply chain event timeline for a batch, ordered by EventTime ascending.
    /// Authorization: ADMIN | MANAGER | INSPECTOR | PUBLIC
    /// </summary>
    [HttpGet("api/v1/batches/{batchId:guid}/events")]
    [ProducesResponseType(typeof(IReadOnlyList<SupplyChainEventResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEventsByBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var dtos = await _mediator.Send(
            new GetEventsByBatchQuery(batchId),
            cancellationToken);

        return Ok(ApiResponse.Success(dtos.Select(ToResponse).ToList()));
    }

    // ─────────────────────────────────────────────────────────────
    // GET /api/v1/events/{eventId}
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Get the details of a specific supply chain event.
    /// Authorization: ADMIN | MANAGER | INSPECTOR
    /// </summary>
    [HttpGet("api/v1/events/{eventId:guid}")]
    [ProducesResponseType(typeof(SupplyChainEventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventById(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(
            new GetSupplyChainEventByIdQuery(eventId),
            cancellationToken);

        if (dto is null)
        {
            return NotFound(
                ApiResponse.Fail(
                    HttpStatusCode.NotFound,
                    $"Event '{eventId}' was not found."));
        }

        return Ok(ApiResponse.Success(ToResponse(dto)));
    }

    // ─────────────────────────────────────────────────────────────
    // GET /api/v1/batches/{batchId}/events/verify
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Verify the integrity of the hash chain for a batch.
    /// Returns { "isValid": true } if the chain has not been tampered with.
    /// Authorization: ADMIN | INSPECTOR
    /// </summary>
    [HttpGet("api/v1/batches/{batchId:guid}/events/verify")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyHashChain(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var isValid = await _mediator.Send(
            new VerifyHashChainQuery(batchId),
            cancellationToken);

        return Ok(new { isValid });
    }

    // ─────────────────────────────────────────────────────────────
    // Mapping helper
    // ─────────────────────────────────────────────────────────────

    private static SupplyChainEventResponse ToResponse(
        AgriTrace.Application.Contracts.SupplyChainEventDto dto) => new()
    {
        EventId = dto.Id,
        BatchId = dto.BatchId,
        EventTypeId = dto.EventTypeId,
        OrganizationId = dto.OrganizationId,
        PerformedByUserId = dto.PerformedByUserId,
        EventData = dto.EventData,
        Location = dto.Location,
        PreviousHash = dto.PreviousHash,
        CurrentHash = dto.CurrentHash,
        EventTime = dto.EventTime,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt
    };
}
