using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities.Notifications;
using AgriTrace.Domain.Entities.Users;
using AgriTrace.Application.Emails;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
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
        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => r.Equals("MANAGER", StringComparison.OrdinalIgnoreCase)
                    || r.Equals("STAFF", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Role must be MANAGER or STAFF.");
    }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;

    public CreateUserCommandHandler(
        IUserService userService,
        ICurrentUserService currentUser,
        IEmailService emailService,
        INotificationService notificationService)
    {
        _userService = userService;
        _currentUser = currentUser;
        _emailService = emailService;
        _notificationService = notificationService;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("Authentication required to create users.");
        }

        var isAdmin = string.Equals(_currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase);

        if (!isAdmin && !string.Equals(_currentUser.Role, "Manager", StringComparison.OrdinalIgnoreCase))
        {
            throw new RbacForbiddenException("RBAC_INVALID_ROLE", "Only ADMIN or MANAGER can create users.");
        }

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role)
            || !Enum.IsDefined(typeof(UserRole), role))
        {
            throw new ArgumentException($"Role '{request.Role}' is invalid.");
        }

        // MANAGER can only create STAFF users
        if (!isAdmin && role != UserRole.Staff)
        {
            throw new RbacForbiddenException("RBAC_INVALID_ROLE", "MANAGER can only invite STAFF.");
        }

        var email = request.Email.Trim().ToLowerInvariant();

        var existing = await _userService.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
        {
            throw new ConflictException($"Email '{email}' already exists.");
        }

        Guid organizationId;
        if (isAdmin)
        {
            organizationId = request.OrganizationId
                ?? _currentUser.OrganizationId
                ?? new Guid("50000000-0000-0000-0000-000000000001");
        }
        else
        {
            organizationId = _currentUser.OrganizationId
                ?? request.OrganizationId
                ?? new Guid("50000000-0000-0000-0000-000000000001");
        }

        var user = new User(
            organizationId,
            request.FullName,
            email,
            User.HashPassword(request.Password),
            role);

        var created = await _userService.CreateAsync(user, cancellationToken);

        var (subject, body) = WelcomeEmailTemplate.Build(
            request.FullName, email, request.Password);
        await _emailService.SendAsync(email, subject, body, cancellationToken);

        var titleJson = System.Text.Json.JsonSerializer.Serialize(new { en = "User Created", vi = "Tài khoản mới" });
        var msgJson = System.Text.Json.JsonSerializer.Serialize(new { 
            en = $"User '{request.FullName}' ({email}) has been created with role {request.Role}.",
            vi = $"Tài khoản '{request.FullName}' ({email}) đã được tạo với vai trò {request.Role}."
        });
        
        var notification = new Notification(
            _currentUser.UserId,
            titleJson,
            msgJson);
        await _notificationService.CreateAsync(notification, cancellationToken);

        return ToDto(created);
    }

    public static UserDto ToDto(User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        Role = user.Role.ToString(),
        OrganizationId = user.OrganizationId ?? Guid.Empty,
        OrganizationName = user.Organization?.Name ?? string.Empty,
        OrganizationTypeName = user.Organization?.OrganizationType?.Name ?? string.Empty,
        Phone = user.Phone ?? string.Empty,
        IsActive = user.IsActive,
        Status = user.Status,
        MustChangePassword = user.MustChangePassword,
        CreatedAt = user.CreatedAt
    };
}

