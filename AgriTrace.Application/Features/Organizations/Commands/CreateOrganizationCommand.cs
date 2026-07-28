using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
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
        if (_currentUser.Role != "Admin")
        {
            throw new RbacForbiddenException("RBAC_INVALID_ROLE", "Only ADMIN can create organizations.");
        }
        var organizationType = await _organizationTypeService.GetByIdAsync(request.OrganizationTypeId, cancellationToken);
        if (organizationType == null) {
            throw new ConflictException("OrganizationType is not exist!");
        }
            

        if (organizationType.Code == "SYSTEM")
        {
            throw new RbacForbiddenException("RBAC_ORG_PROHIBITED", "Creating SYSTEM organizations is forbidden via public API.");
        }

        if (await _organizationService.GetByNameAsync(request.Name, cancellationToken) != null)
            throw new ConflictException("Organization name already exists.");

        var organization = new Organization(request.OrganizationTypeId, request.Name, request.Address);
        var created = await _organizationService.CreateAsync(organization, cancellationToken);

        return created.Adapt<OrganizationDto>();
    }


}

public sealed class CreateOrganizationCommandValidator
    : AbstractValidator<CreateOrganizationCommand>
{
    private readonly IOrganizationTypeService _organizationTypeService;
    public CreateOrganizationCommandValidator(IOrganizationTypeService organizationTypeService)
    {
        _organizationTypeService = organizationTypeService;
        RuleFor(x => x.OrganizationTypeId)
            .NotEmpty();
        RuleFor(x => x.OrganizationTypeId)
            .MustAsync(NotBeSystemType)                          // (1)
            .WithMessage("SYSTEM organization type is prohibited.") // (2)
            .When(x => x.OrganizationTypeId != Guid.Empty);
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
    private async Task<bool> NotBeSystemType(Guid organizationTypeId, CancellationToken cancellationToken)
    {
        var organizationType = await _organizationTypeService.GetByIdAsync(organizationTypeId, cancellationToken);
        return organizationType is null || !string.Equals(organizationType.Code, "SYSTEM", StringComparison.OrdinalIgnoreCase);
    }

}
