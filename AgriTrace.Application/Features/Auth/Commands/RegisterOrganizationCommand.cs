using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Application.Emails;
using AgriTrace.Application.Features.Users.Commands;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using FluentValidation;
using MediatR;

namespace AgriTrace.Application.Features.Auth.Commands;

public record RegisterOrganizationCommand(
    string FullName,
    string Email,
    string Password,
    string OrgName,
    string? OrgAddress,
    Guid OrgTypeId) : IRequest<UserDto>;

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
    private readonly IOrganizationService _organizationService;
    private readonly IEmailService _emailService;

    public RegisterOrganizationCommandHandler(
        IUserService userService,
        IOrganizationService organizationService,
        IEmailService emailService)
    {
        _userService = userService;
        _organizationService = organizationService;
        _emailService = emailService;
    }

    public async Task<UserDto> Handle(RegisterOrganizationCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // 1. Validate trước — không tạo gì nếu email đã tồn tại
        if (await _userService.GetByEmailAsync(email, cancellationToken) is not null)
            throw new ConflictException($"Email '{email}' đã được sử dụng.");

        // 2. Tạo org
        var existingOrg = await _organizationService.GetByNameAsync(request.OrgName, cancellationToken);
        if (existingOrg is not null)
            throw new ConflictException("Tên tổ chức đã tồn tại. Vui lòng chọn tên khác.");

        var org = new Organization(request.OrgTypeId, request.OrgName, request.OrgAddress);
        var createdOrg = await _organizationService.CreateAsync(org, cancellationToken);

        // 3. Tạo user
        var user = new User(
            createdOrg.Id,
            request.FullName,
            email,
            User.HashPassword(request.Password),
            UserRole.Manager,
            isActive: false);

        var created = await _userService.CreateAsync(user, cancellationToken);

        var (subject, body) = WelcomeEmailTemplate.Build(request.FullName, email, request.Password);
        await _emailService.SendAsync(email, subject, body, cancellationToken);

        return CreateUserCommandHandler.ToDto(created);
    }
}