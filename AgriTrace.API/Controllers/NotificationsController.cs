using AgriTrace.API.Models;
using AgriTrace.API.Models.Notifications;
using AgriTrace.API.Services;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Notifications.Commands;
using AgriTrace.Application.Features.Notifications.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Quản lý thông báo của người dùng.
/// </summary>
[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public sealed class NotificationsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public NotificationsController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Danh sách thông báo
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetAll(
        bool? isRead,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetNotificationsPagedQuery(_currentUser.UserId, isRead, page, pageSize),
            cancellationToken);

        var paged = new NotificationPagedResponse(
            result.Items.Select(ToItem),
            result.TotalCount,
            result.PageNumber,
            result.PageSize);

        return Ok(ApiResponse.Success(paged));
    }

    /// <summary>
    /// Số thông báo chưa đọc
    /// </summary>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetUnreadCount(
        CancellationToken cancellationToken)
    {
        var count = await _sender.Send(
            new GetUnreadCountQuery(_currentUser.UserId),
            cancellationToken);

        return Ok(ApiResponse.Success(new { unreadCount = count }));
    }

    /// <summary>
    /// Đánh dấu tất cả thông báo đã đọc
    /// </summary>
    [HttpPatch("read-all")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> MarkAllRead(
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new MarkAllNotificationsReadCommand(_currentUser.UserId),
            cancellationToken);

        return Ok(ApiResponse.Success(null, "Đã đánh dấu tất cả là đã đọc"));
    }

    /// <summary>
    /// Đánh dấu thông báo đã đọc
    /// </summary>
    [HttpPatch("{notificationId:guid}/read")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> MarkRead(
        Guid notificationId,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new MarkNotificationReadCommand(notificationId, _currentUser.UserId),
            cancellationToken);

        return Ok(ApiResponse.Success(null, "Đã đánh dấu là đã đọc"));
    }

    private static NotificationItem ToItem(NotificationDto dto) => new()
    {
        NotificationId = dto.NotificationId,
        UserId = dto.UserId,
        Title = dto.Title,
        Message = dto.Message,
        IsRead = dto.IsRead,
        CreatedAt = dto.CreatedAt
    };
}
