using System.Security.Claims;
using AgriTrace.API.Models;
using AgriTrace.API.Models.Users;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Users.Commands;
using AgriTrace.Application.Features.Users.Queries;
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

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lấy danh sách người dùng
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetAll(
        Guid? organizationId,
        string? role,
        string? search,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetUsersPagedQuery(organizationId, role, search, page, pageSize),
            cancellationToken);

        var paged = new UserPagedResponse(
            result.Items.Select(ToListItem),
            result.TotalCount,
            result.PageNumber,
            result.PageSize);

        return Ok(ApiResponse.Success(paged));
    }

    /// <summary>
    /// Tạo User mới
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateUserCommand(
                request.OrganizationId,
                request.FullName,
                request.Email,
                request.Password,
                request.Role),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { userId = result.Id },
            ApiResponse.Success(new { userId = result.Id }, "User created successfully."));
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
            "Profile updated successfully."));
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
    [Authorize(Roles = "Admin")]
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
            "User updated successfully."));
    }

    /// <summary>
    /// Kích hoạt / Vô hiệu hóa tài khoản
    /// </summary>
    [HttpPatch("{userId:guid}/status")]
    [Authorize(Roles = "Admin")]
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
            "User status updated successfully."));
    }

    private static UserListItem ToListItem(UserDto dto) => new()
    {
        UserId = dto.Id,
        FullName = dto.FullName,
        Email = dto.Email,
        Role = dto.Role,
        OrganizationId = dto.OrganizationId,
        OrganizationName = dto.OrganizationName,
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
        OrganizationName = dto.OrganizationName,
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
