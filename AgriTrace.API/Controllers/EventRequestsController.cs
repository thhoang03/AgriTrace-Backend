using AgriTrace.API.Models;
using AgriTrace.API.Models.EventRequests;
using AgriTrace.Application.Features.EventRequests.Commands;
using AgriTrace.Application.Features.EventRequests.Queries;
using AgriTrace.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Quản lý Yêu cầu Ghi nhận Sự kiện (Event Requests).
/// </summary>
[ApiController]
[Route("api/v1/event-requests")]
[Authorize]
public sealed class EventRequestsController : ControllerBase
{
    private readonly ISender _sender;

    public EventRequestsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Danh sách Yêu cầu Sự kiện
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetEventRequests(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? batchId = null,
        [FromQuery] EventRequestStatus? status = null,
        [FromQuery] bool onlyMine = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetEventRequestsQuery(page, pageSize, batchId, status, onlyMine),
            cancellationToken);

        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// Tạo Yêu cầu Sự kiện mới
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> CreateEventRequest(
        [FromBody] CreateEventRequestModel model,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new CreateEventRequestCommand(
                model.BatchId,
                model.EventTypeId,
                model.Location,
                model.Description,
                model.EventData
            ),
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse.Success(result, "Tạo yêu cầu sự kiện thành công"));
    }

    /// <summary>
    /// Phê duyệt Yêu cầu Sự kiện
    /// </summary>
    [HttpPut("{id:guid}/approve")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> ApproveEventRequest(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new ApproveEventRequestCommand(id),
            cancellationToken);

        return Ok(ApiResponse.Success(result, "Phê duyệt yêu cầu thành công"));
    }

    /// <summary>
    /// Từ chối Yêu cầu Sự kiện
    /// </summary>
    [HttpPut("{id:guid}/reject")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> RejectEventRequest(
        Guid id,
        [FromBody] RejectEventRequestModel? model,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new RejectEventRequestCommand(id, model?.Reason),
            cancellationToken);

        return Ok(ApiResponse.Success(result, "Từ chối yêu cầu thành công"));
    }
}
