using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
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
using FluentValidation;
using MediatR;

namespace AgriTrace.Application.Features.Users.Commands;

public record CreateUserCommand(
    Guid? OrganizationId,
    string FullName,
    string Email,
    string Password,
    string Role) : IRequest<UserDto>;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Role).NotEmpty();
    }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserService _userService;

    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role)
            || !Enum.IsDefined(typeof(UserRole), role))
        {
            throw new ArgumentException($"Role '{request.Role}' is invalid.");
        }

        var email = request.Email.Trim().ToLowerInvariant();

        var existing = await _userService.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
        {
            throw new ConflictException($"Email '{email}' already exists.");
        }

        var user = new User(
            request.OrganizationId,
            request.FullName,
            email,
            User.HashPassword(request.Password),
            role);

        var created = await _userService.CreateAsync(user, cancellationToken);

        return ToDto(created);
    }

    public static UserDto ToDto(User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        Role = user.Role.ToString(),
        OrganizationId = user.OrganizationId ?? Guid.Empty,
        Phone = user.Phone ?? string.Empty,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt
    };
}

