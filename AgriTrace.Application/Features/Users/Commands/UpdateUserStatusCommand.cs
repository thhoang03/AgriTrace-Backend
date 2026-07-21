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

    public UpdateUserStatusCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserDto> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} not found.");

        user.SetChangeStatus(request.IsActive);
        await _userService.UpdateAsync(user, cancellationToken);

        return CreateUserCommandHandler.ToDto(user);
    }
}
