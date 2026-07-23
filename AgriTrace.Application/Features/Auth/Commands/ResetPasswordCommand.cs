using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Auth.Commands;

// STUB: password reset. Validates the persisted reset token and updates the password.
public record ResetPasswordCommand(
    string Token,
    string NewPassword) : IRequest<MediatR.Unit>;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, MediatR.Unit>
{
    private readonly IUserService _userService;

    public ResetPasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<MediatR.Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByResetTokenAsync(request.Token, cancellationToken)
            ?? throw new ArgumentException("Token đặt lại mật khẩu không hợp lệ.");

        if (!user.IsResetPasswordTokenValid(request.Token))
        {
            throw new ArgumentException("Token đặt lại mật khẩu đã hết hạn.");
        }

        user.SetPassword(request.NewPassword);
        user.ClearResetPasswordToken();
        await _userService.UpdateAsync(user, cancellationToken);

        return MediatR.Unit.Value;
    }
}
