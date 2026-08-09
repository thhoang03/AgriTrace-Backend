using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Emails;
using AgriTrace.Application.Features.Users.Commands;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using FluentValidation;
using MediatR;

namespace AgriTrace.Application.Features.Auth.Commands;

public record RegisterOrganizationCommand(
    Guid OrganizationId,
    string FullName,
    string Email,
    string Password) : IRequest<UserDto>;

public class RegisterOrganizationCommandValidator : AbstractValidator<RegisterOrganizationCommand>
{
    public RegisterOrganizationCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public class RegisterOrganizationCommandHandler : IRequestHandler<RegisterOrganizationCommand, UserDto>
{
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;

    public RegisterOrganizationCommandHandler(IUserService userService, IEmailService emailService)
    {
        _userService = userService;
        _emailService = emailService;
    }

    public async Task<UserDto> Handle(RegisterOrganizationCommand request, CancellationToken cancellationToken)
    {
        // KHÔNG check _currentUser — đây là bootstrap, chưa có ai đăng nhập
        var email = request.Email.Trim().ToLowerInvariant();

        var existing = await _userService.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
            throw new ConflictException($"Email '{email}' already exists.");

        var user = new User(
            request.OrganizationId,
            request.FullName,
            email,
            User.HashPassword(request.Password),
            UserRole.Manager); // fix cứng Manager, không nhận Role từ client

        var created = await _userService.CreateAsync(user, cancellationToken);

        var (subject, body) = WelcomeEmailTemplate.Build(request.FullName, email, request.Password);
        await _emailService.SendAsync(email, subject, body, cancellationToken);

        return CreateUserCommandHandler.ToDto(created);
    }
}