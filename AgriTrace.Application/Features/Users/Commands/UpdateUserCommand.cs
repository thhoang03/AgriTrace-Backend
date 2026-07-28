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
using AgriTrace.Domain.Interfaces.Outbound;
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
    private readonly ICurrentUserService _currentUser;

    public UpdateUserCommandHandler(
        IUserService userService,
        ICurrentUserService currentUser)
    {
        _userService = userService;
        _currentUser = currentUser;
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

            if (parsed == UserRole.Manager
                && user.Role != UserRole.Manager
                && !string.Equals(_currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new RbacForbiddenException(
                    "RBAC_ROLE_UPGRADE",
                    "Chỉ Admin mới có thể thăng cấp người dùng lên Manager.");
            }

            role = parsed;
        }

        user.UpdateProfileDetails(request.FullName, request.Phone, role);
        await _userService.UpdateAsync(user, cancellationToken);

        return CreateUserCommandHandler.ToDto(user);
    }
}

