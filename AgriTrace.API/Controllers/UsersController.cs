using System.Security.Claims;
using AgriTrace.API.Models;
using AgriTrace.API.Models.Users;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Users.Commands;
using AgriTrace.Application.Features.Users.Queries;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Quản lý User (người dùng).
/// </summary>
[ApiController]
[Route("api/v1/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public UsersController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Lấy danh sách người dùng
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetAll(
        string? role,
        string? search,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Guid? orgId = _currentUser.OrganizationId;

        if (!IsAdmin)
        {
            if (!orgId.HasValue)
            {
                var empty = new UserPagedResponse([], 0, page, pageSize);
                return Ok(ApiResponse.Success(empty));
            }
        }
        else
        {
            orgId = null;
        }

        var result = await _sender.Send(
            new GetUsersPagedQuery(orgId, role, search, page, pageSize),
            cancellationToken);

        var paged = new UserPagedResponse(
            result.Items.Select(ToListItem),
            result.TotalCount,
            result.PageNumber,
            result.PageSize);

        return Ok(ApiResponse.Success(paged));
    }

    private bool IsAdmin =>
        string.Equals(_currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase)
     || string.Equals(_currentUser.OrganizationType, "System", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Tạo User mới
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateUserCommand(
                null,
                request.FullName,
                request.Email,
                request.Password,
                request.Role),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { userId = result.Id },
            ApiResponse.Success(new { userId = result.Id }, "Tạo người dùng thành công"));
    }

    /// <summary>
    /// Xem hồ sơ cá nhân
    /// </summary>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetProfile(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetUserProfileQuery(GetCurrentUserId()),
            cancellationToken);

        return Ok(ApiResponse.Success(ToDetail(result)));
    }

    /// <summary>
    /// Cập nhật hồ sơ cá nhân
    /// </summary>
    [HttpPut("profile")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> UpdateProfile(
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateUserCommand(
                GetCurrentUserId(),
                request.FullName,
                request.Phone,
                request.Role),
            cancellationToken);

        return Ok(ApiResponse.Success(
            ToDetail(result),
            "Cập nhật hồ sơ thành công"));
    }

    /// <summary>
    /// Lấy thông tin chi tiết User
    /// </summary>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetById(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetUserByIdQuery(userId),
            cancellationToken);

        return Ok(ApiResponse.Success(ToDetail(result)));
    }

    /// <summary>
    /// Cập nhật User
    /// </summary>
    [HttpPut("{userId:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Update(
        Guid userId,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateUserCommand(
                userId,
                request.FullName,
                request.Phone,
                request.Role),
            cancellationToken);

        return Ok(ApiResponse.Success(
            ToDetail(result),
            "Cập nhật người dùng thành công"));
    }

    /// <summary>
    /// Kích hoạt / Vô hiệu hóa tài khoản
    /// </summary>
    [HttpPatch("{userId:guid}/status")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateStatus(
        Guid userId,
        [FromBody] UserStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateUserStatusCommand(userId, request.IsActive),
            cancellationToken);

        return Ok(ApiResponse.Success(
            ToDetail(result),
            "Cập nhật trạng thái người dùng thành công"));
    }

    /// <summary>
    /// Admin/Manager đặt lại mật khẩu cho user
    /// </summary>
    [HttpPost("{userId:guid}/reset-password")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AdminResetPassword(
        Guid userId,
        [FromBody] AdminResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new AdminResetPasswordCommand(userId, request.NewPassword),
            cancellationToken);

        return Ok(ApiResponse.Success(null, "Đặt lại mật khẩu thành công"));
    }

    private static UserListItem ToListItem(UserDto dto) => new()
    {
        UserId = dto.Id,
        FullName = dto.FullName,
        Email = dto.Email,
        Role = dto.Role,
        OrganizationId = dto.OrganizationId,
        OrganizationName = dto.OrganizationName,
        OrganizationTypeName = dto.OrganizationTypeName,
        IsActive = dto.IsActive,
        CreatedAt = dto.CreatedAt
    };

    private static UserDetail ToDetail(UserDto dto) => new()
    {
        UserId = dto.Id,
        FullName = dto.FullName,
        Email = dto.Email,
        Role = dto.Role,
        OrganizationId = dto.OrganizationId,
        Phone = dto.Phone,
        IsActive = dto.IsActive
    };

    private Guid GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out var id))
        {
            throw new UnauthorizedAccessException("Không xác định được người dùng hiện tại.");
        }

        return id;
    }
}
