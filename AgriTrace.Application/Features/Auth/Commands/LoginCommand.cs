using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts.Auth;
using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.Categories;
using AgriTrace.Domain.Entities.Certificates;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.Notifications;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Entities.Recalls;
using AgriTrace.Domain.Entities.Units;
using AgriTrace.Domain.Entities.Users;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Auth.Commands;

public record LoginCommand(
    string Email,
    string Password) : IRequest<LoginResult>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IUserService userService,
        ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _userService.GetByEmailAsync(email, cancellationToken)
            ?? throw new ArgumentException("Email hoặc mật khẩu không đúng.");

        if (!user.VerifyPassword(request.Password))
        {
            throw new ArgumentException("Email hoặc mật khẩu không đúng.");
        }

        if (!user.IsActive)
        {
            throw new ArgumentException("Tài khoản đã bị vô hiệu hóa.");
        }

        var refreshToken = _tokenService.GenerateRefreshToken();
        var expiryDays = 7;
        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(expiryDays));
        await _userService.UpdateAsync(user, cancellationToken);

        return new LoginResult
        {
            AccessToken = _tokenService.GenerateAccessToken(user),
            RefreshToken = refreshToken,
            User = new UserBasicInfo
            {
                Id = user.Id,
                Name = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            }
        };
    }
}

