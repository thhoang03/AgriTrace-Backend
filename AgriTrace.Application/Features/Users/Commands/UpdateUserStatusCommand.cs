using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Users.Commands;

public record UpdateUserStatusCommand(
    Guid UserId,
    bool IsActive) : IRequest<UserDto>;

public class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand, UserDto>
{
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;

    public UpdateUserStatusCommandHandler(IUserService userService,ICurrentUserService currentUser)
    {
        _userService = userService;
        _currentUser = currentUser;
    }

    public async Task<UserDto> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} not found.");
        if (request.UserId == _currentUser.UserId)
        {
            throw new RbacForbiddenException(
            "RBAC_SELF_STATUS_CHANGE",
            "Không thể tự deactive chính mình.");
        }
        user.SetChangeStatus(request.IsActive);
        await _userService.UpdateAsync(user, cancellationToken);

        return CreateUserCommandHandler.ToDto(user);
    }
}
