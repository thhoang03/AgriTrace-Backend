using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Auth.Commands;

// STUB: email sending is out of scope (Phase 7 Excluded). We generate and persist a reset token;
// actual email delivery is a TODO.
public record ForgotPasswordCommand(
    string Email) : IRequest<MediatR.Unit>;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, MediatR.Unit>
{
    private readonly IUserService _userService;

    public ForgotPasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<MediatR.Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _userService.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            // Do not reveal whether the email exists.
            return MediatR.Unit.Value;
        }

        var resetToken = Convert.ToBase64String(
            System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));

        user.SetResetPasswordToken(resetToken, DateTime.UtcNow.AddHours(1));
        await _userService.UpdateAsync(user, cancellationToken);

        // TODO: send resetToken via email (Phase 7 stub — not implemented).

        return MediatR.Unit.Value;
    }
}
