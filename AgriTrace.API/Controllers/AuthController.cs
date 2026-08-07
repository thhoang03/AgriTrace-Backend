using System.Security.Claims;
using AgriTrace.API.Models;
using AgriTrace.API.Models.Auth;
using AgriTrace.API.Models.Users;
using AgriTrace.Application.Contracts.Auth;
using AgriTrace.Application.Features.Auth.Commands;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Xác thực và quản lý phiên người dùng (JWT).
/// </summary>
[ApiController]
[Route("api/v1/auth")]
[Authorize]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IOrganizationService _organizationService;

    public AuthController(ISender sender, IOrganizationService organizationService)
    {
        _sender = sender;
        _organizationService = organizationService;
    }

    /// <summary>
    /// Đăng ký tài khoản doanh nghiệp mới
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        // 1. Resolve Organization Type
        Guid orgTypeId = request.OrganizationTypeId ?? Guid.Empty;
        if (orgTypeId == Guid.Empty && !string.IsNullOrEmpty(request.OrganizationTypeCode))
        {
            var types = await _sender.Send(new Application.Features.Lookup.Queries.GetOrganizationTypesQuery(), cancellationToken);
            var found = types.FirstOrDefault(t => string.Equals(t.Code, request.OrganizationTypeCode, StringComparison.OrdinalIgnoreCase));
            if (found != null && Guid.TryParse(found.Id, out var parsedGuid))
            {
                orgTypeId = parsedGuid;
            }
        }
        if (orgTypeId == Guid.Empty)
        {
            orgTypeId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        }

        // 2. Create Organization if name provided
        Guid organizationId;
        string orgName = string.IsNullOrWhiteSpace(request.OrganizationName) ? $"{request.FullName}'s Organization" : request.OrganizationName;
        
        var existingOrg = await _organizationService.GetByNameAsync(orgName, cancellationToken);
        if (existingOrg != null)
        {
            organizationId = existingOrg.Id;
        }
        else
        {
            var newOrg = new Organization(orgTypeId, orgName, request.OrganizationAddress);
            var createdOrg = await _organizationService.CreateAsync(newOrg, cancellationToken);
            organizationId = createdOrg.Id;
        }

        // 3. Create User account (Manager role)
        await _sender.Send(
            new Application.Features.Users.Commands.CreateUserCommand(
                organizationId,
                request.FullName,
                request.Email,
                request.Password,
                "Manager"
            ),
            cancellationToken);

        // 4. Return success response (do not auto-login)
        return Ok(ApiResponse.Success(null, "Đăng ký tài khoản doanh nghiệp thành công. Tài khoản của bạn đang chờ quản trị viên phê duyệt."));
    }

    /// <summary>
    /// Đăng nhập
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new LoginCommand(request.Email, request.Password),
            cancellationToken);

        var loginData = new LoginData
        {
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken,
            MustChangePassword = result.MustChangePassword,
            User = new UserBasic
            {
                Id = result.User.Id,
                Name = result.User.Name,
                Email = result.User.Email,
                Role = result.User.Role,
                OrganizationType = result.User.OrganizationType,
                MustChangePassword = result.User.MustChangePassword
            }
        };

        return Ok(ApiResponse.Success(loginData));
    }

    /// <summary>
    /// Làm mới Access Token
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new RefreshTokenCommand(request.RefreshToken),
            cancellationToken);

        var tokenPair = new TokenPair
        {
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken
        };

        return Ok(ApiResponse.Success(tokenPair));
    }

    /// <summary>
    /// Đăng xuất
    /// </summary>
    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {
        // Refresh-token revocation is deferred to auth enforcement (Phase 10).
        return NoContent();
    }

    /// <summary>
    /// Lấy thông tin User hiện tại
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse> Me()
    {
        var id = GetCurrentUserId();
        var user = new UserBasic
        {
            Id = id,
            Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            Name = User.Identity?.Name ?? string.Empty,
            Role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty
        };

        return Ok(ApiResponse.Success(user));
    }

    /// <summary>
    /// Đổi mật khẩu
    /// </summary>
    [HttpPut("change-password")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ChangePasswordCommand(
                GetCurrentUserId(),
                request.CurrentPassword,
                request.NewPassword),
            cancellationToken);

        return Ok(ApiResponse.Success(
            null,
            "Đổi mật khẩu thành công"));
    }

    /// <summary>
    /// Quên mật khẩu
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ForgotPasswordCommand(request.Email),
            cancellationToken);

        return Ok(ApiResponse.Success(
            null,
            "Nếu email tồn tại, hướng dẫn đặt lại mật khẩu đã được gửi."));
    }

    /// <summary>
    /// Đặt lại mật khẩu
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ResetPasswordCommand(request.Token, request.NewPassword),
            cancellationToken);

        return Ok(ApiResponse.Success(
            null,
            "Đặt lại mật khẩu thành công"));
    }

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
