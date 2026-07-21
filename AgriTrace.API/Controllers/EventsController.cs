using AgriTrace.API.Models;
using AgriTrace.API.Models.Events;
using AgriTrace.API.Services;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Events.Commands;
using AgriTrace.Application.Features.Events.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Quản lý Supply Chain Event của Batch.
/// </summary>
[ApiController]
[Authorize]
public sealed class EventsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public EventsController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Danh sách Event của Batch
    /// </summary>
    [HttpGet("api/v1/batches/{batchId:guid}/events")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetBatchEvents(
        Guid batchId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetBatchEventsQuery(batchId, page, pageSize),
            cancellationToken);

        var paged = new EventPagedResponse(
            result.Items.Select(ToListItem),
            result.TotalCount,
            result.PageNumber,
            result.PageSize);

        return Ok(ApiResponse.Success(paged));
    }

    /// <summary>
    /// Ghi nhận Supply Chain Event
    /// </summary>
    [HttpPost("api/v1/batches/{batchId:guid}/events")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> CreateEvent(
        Guid batchId,
        [FromBody] CreateEventRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateEventCommand(
                batchId,
                request.EventTypeId,
                request.EventData,
                request.Location,
                _currentUser.UserId),
            cancellationToken);

        var data = new EventCreatedData
        {
            EventId = result.EventId,
            PreviousHash = result.PreviousHash,
            CurrentHash = result.CurrentHash
        };

        return CreatedAtAction(
            nameof(GetEventById),
            new { eventId = result.EventId },
            ApiResponse.Success(data, "Tạo sự kiện thành công"));
    }

    /// <summary>
    /// Kiểm tra Hash Chain của Batch
    /// </summary>
    [HttpGet("api/v1/batches/{batchId:guid}/events/verify")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> VerifyHashChain(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new VerifyHashChainQuery(batchId),
            cancellationToken);

        var data = new HashChainVerifyResponse
        {
            IsValid = result.IsValid,
            TotalEvents = result.TotalEvents
        };

        return Ok(ApiResponse.Success(data));
    }

    /// <summary>
    /// Chi tiết Event
    /// </summary>
    [HttpGet("api/v1/events/{eventId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetEventById(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetEventByIdQuery(eventId),
            cancellationToken);

        return Ok(ApiResponse.Success(ToDetail(result)));
    }

    private static EventListItem ToListItem(EventDto dto) => new()
    {
        EventId = dto.EventId,
        BatchId = dto.BatchId,
        EventTypeId = dto.EventTypeId,
        EventTypeCode = dto.EventTypeCode,
        OrganizationId = dto.OrganizationId,
        PerformedByUserId = dto.PerformedByUserId,
        EventData = dto.EventData,
        Location = dto.Location,
        PreviousHash = dto.PreviousHash,
        CurrentHash = dto.CurrentHash,
        EventTime = dto.EventTime
    };

    private static EventDetail ToDetail(EventDto dto) => new()
    {
        EventId = dto.EventId,
        BatchId = dto.BatchId,
        EventTypeId = dto.EventTypeId,
        EventTypeCode = dto.EventTypeCode,
        OrganizationId = dto.OrganizationId,
        PerformedByUserId = dto.PerformedByUserId,
        EventData = dto.EventData,
        Location = dto.Location,
        PreviousHash = dto.PreviousHash,
        CurrentHash = dto.CurrentHash,
        EventTime = dto.EventTime
    };
}
