using AgriTrace.API.Models;
using AgriTrace.API.Models.Recalls;
using AgriTrace.API.Services;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Recalls.Commands;
using AgriTrace.Application.Features.Recalls.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Quản lý Recall (thu hồi Batch).
/// </summary>
[ApiController]
[Route("api/v1/recalls")]
[Authorize]
public sealed class RecallsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public RecallsController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Danh sách Recall
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetAll(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetRecallsPagedQuery(page, pageSize),
            cancellationToken);

        var paged = new RecallPagedResponse(
            result.Items.Select(ToDetail),
            result.TotalCount,
            result.PageNumber,
            result.PageSize);

        return Ok(ApiResponse.Success(paged));
    }

    /// <summary>
    /// Tạo Recall (thu hồi Batch)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse>> Create(
        [FromBody] CreateRecallRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateRecallCommand(
                request.BatchId,
                request.Reason,
                request.Severity,
                _currentUser.UserId),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { recallId = result.RecallId },
            ApiResponse.Success(new { recallId = result.RecallId }, "Tạo thu hồi thành công"));
    }

    /// <summary>
    /// Chi tiết Recall
    /// </summary>
    [HttpGet("{recallId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetById(
        Guid recallId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetRecallByIdQuery(recallId),
            cancellationToken);

        return Ok(ApiResponse.Success(ToDetail(result)));
    }

    /// <summary>
    /// Kết thúc Recall
    /// </summary>
    [HttpPatch("{recallId:guid}/resolve")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Resolve(
        Guid recallId,
        [FromBody] ResolveRecallRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ResolveRecallCommand(recallId, request.Status, _currentUser.UserId),
            cancellationToken);

        return Ok(ApiResponse.Success(null, "Cập nhật thu hồi thành công"));
    }

    private static RecallDetail ToDetail(RecallDto dto) => new()
    {
        RecallId = dto.RecallId,
        BatchId = dto.BatchId,
        BatchCode = dto.BatchCode,
        CreatedBy = dto.CreatedBy,
        CreatedByName = dto.CreatedByName,
        Reason = dto.Reason,
        Severity = dto.Severity,
        SeverityName = dto.SeverityName,
        Status = dto.Status,
        StatusName = dto.StatusName,
        CreatedAt = dto.CreatedAt
    };
}
