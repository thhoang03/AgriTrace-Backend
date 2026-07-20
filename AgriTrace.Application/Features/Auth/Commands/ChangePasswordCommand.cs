using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Auth.Commands;

public record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword) : IRequest<MediatR.Unit>;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, MediatR.Unit>
{
    private readonly IUserService _userService;

    public ChangePasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<MediatR.Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} not found.");

        if (!user.VerifyPassword(request.CurrentPassword))
        {
            throw new ArgumentException("Mật khẩu hiện tại không đúng.");
        }

        user.SetPassword(request.NewPassword);
        await _userService.UpdateAsync(user, cancellationToken);

        return MediatR.Unit.Value;
    }
}
