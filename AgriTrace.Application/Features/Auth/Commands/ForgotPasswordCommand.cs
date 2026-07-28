using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Emails;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using MediatR;

namespace AgriTrace.Application.Features.Auth.Commands;

public record ForgotPasswordCommand(
    string Email) : IRequest<MediatR.Unit>;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, MediatR.Unit>
{
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(
        IUserService userService,
        IEmailService emailService)
    {
        _userService = userService;
        _emailService = emailService;
    }

    public async Task<MediatR.Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _userService.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            return MediatR.Unit.Value;
        }

        var resetToken = Convert.ToBase64String(
            System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));

        user.SetResetPasswordToken(resetToken, DateTime.UtcNow.AddHours(1));
        await _userService.UpdateAsync(user, cancellationToken);

        var (subject, body) = PasswordResetEmailTemplate.Build(user.FullName, resetToken);
        await _emailService.SendAsync(email, subject, body, cancellationToken);

        return MediatR.Unit.Value;
    }
}
