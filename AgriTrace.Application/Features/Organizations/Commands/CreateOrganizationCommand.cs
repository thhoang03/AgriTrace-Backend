using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Domain.Services;
using FluentValidation;
using Mapster;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace AgriTrace.Application.Features.Organizations.Commands;

public record CreateOrganizationCommand(
    [Required] Guid OrganizationTypeId,
    [Required][StringLength(200, MinimumLength = 1)] string Name,
    [StringLength(500)] string? Address) : IRequest<OrganizationDto>;

public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, OrganizationDto>
{
    private readonly IOrganizationService _organizationService;
    private readonly IOrganizationTypeService _organizationTypeService;
    private readonly ICurrentUserService _currentUser;

    public CreateOrganizationCommandHandler(
        IOrganizationService organizationService,
        ICurrentUserService currentUser,
        IOrganizationTypeService organizationTypeService)
                

    {
        _organizationService = organizationService;
        _currentUser = currentUser;
        _organizationTypeService = organizationTypeService;
    }

    public async Task<OrganizationDto> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        if (!string.Equals(_currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(_currentUser.OrganizationType, "System", StringComparison.OrdinalIgnoreCase))
        {
            throw new RbacForbiddenException("RBAC_INVALID_ROLE", "Only ADMIN can create organizations.");
        }
        var organizationType = await _organizationTypeService.GetByIdAsync(request.OrganizationTypeId, cancellationToken);
        if (organizationType == null) {
            throw new ConflictException("OrganizationType is not exist!");
        }

        if (string.Equals(organizationType.Code, "SYSTEM", StringComparison.OrdinalIgnoreCase))
        {
            throw new RbacForbiddenException("RBAC_ORG_PROHIBITED", "Creating SYSTEM organizations is forbidden via public API.");
        }

        if (await _organizationService.GetByNameAsync(request.Name, cancellationToken) != null)
            throw new ConflictException("Organization name already exists.");

        var organization = new Organization(request.OrganizationTypeId, request.Name, request.Address);
        var created = await _organizationService.CreateAsync(organization, cancellationToken);

        var dto = created.Adapt<OrganizationDto>();
        dto.OrganizationTypeCode = organizationType.Code;
        dto.OrganizationTypeName = organizationType.Name;
        return dto;
    }


}

public sealed class CreateOrganizationCommandValidator
    : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationCommandValidator()
    {
        RuleFor(x => x.OrganizationTypeId)
            .NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
