using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Domain.Entities.Users;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Users.Commands;

public record AdminResetPasswordCommand(
    Guid UserId,
    string NewPassword) : IRequest<MediatR.Unit>;

public class AdminResetPasswordCommandHandler : IRequestHandler<AdminResetPasswordCommand, MediatR.Unit>
{
    private readonly IUserService _userService;

    public AdminResetPasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<MediatR.Unit> Handle(AdminResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} not found.");

        user.SetPassword(request.NewPassword);
        user.MarkPasswordChanged();
        await _userService.UpdateAsync(user, cancellationToken);

        return MediatR.Unit.Value;
    }
}
