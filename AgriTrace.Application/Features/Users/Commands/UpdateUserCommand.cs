using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Features.Users.Commands;
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

namespace AgriTrace.Application.Features.Users.Commands;

public record UpdateUserCommand(
    Guid UserId,
    string? FullName,
    string? Phone,
    string? Role) : IRequest<UserDto>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUserService _userService;

    public UpdateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} not found.");

        UserRole? role = null;
        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var parsed)
                || !Enum.IsDefined(typeof(UserRole), parsed))
            {
                throw new ArgumentException($"Role '{request.Role}' is invalid.");
            }

            role = parsed;
        }

        user.UpdateProfileDetails(request.FullName, request.Phone, role);
        await _userService.UpdateAsync(user, cancellationToken);

        return CreateUserCommandHandler.ToDto(user);
    }
}

