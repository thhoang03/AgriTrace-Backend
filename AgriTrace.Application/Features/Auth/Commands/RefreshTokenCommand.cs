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

public record RefreshTokenCommand(
    string RefreshToken) : IRequest<TokenPairResult>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenPairResult>
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(
        IUserService userService,
        ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    public async Task<TokenPairResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken)
            ?? throw new ArgumentException("Refresh token không hợp lệ.");

        if (!user.IsRefreshTokenValid(request.RefreshToken))
        {
            throw new ArgumentException("Refresh token đã hết hạn hoặc không hợp lệ.");
        }

        if (!user.IsActive)
        {
            throw new ArgumentException("Tài khoản đã bị vô hiệu hóa.");
        }

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        await _userService.UpdateAsync(user, cancellationToken);

        return new TokenPairResult
        {
            AccessToken = _tokenService.GenerateAccessToken(user),
            RefreshToken = newRefreshToken
        };
    }
}

