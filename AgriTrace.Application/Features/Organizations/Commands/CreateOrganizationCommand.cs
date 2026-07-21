using System.ComponentModel.DataAnnotations;
using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using FluentValidation;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Organizations.Commands;

public record CreateOrganizationCommand(
    [Required] string Type,
    [Required][StringLength(200, MinimumLength = 1)] string Name,
    [StringLength(500)] string? Address) : IRequest<OrganizationDto>;

public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, OrganizationDto>
{
    private readonly IOrganizationService _organizationService;

    public CreateOrganizationCommandHandler(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    public async Task<OrganizationDto> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        if (await _organizationService.GetByNameAsync(request.Name, cancellationToken) != null)
            throw new ConflictException("Organization name already exists.");

        // TODO Phase 8: resolve OrganizationType by code from IOrganizationTypeService.
        var organizationTypeId = Guid.NewGuid();

        var organization = new Organization(organizationTypeId, request.Name, request.Address);
        var created = await _organizationService.CreateAsync(organization, cancellationToken);

        return created.Adapt<OrganizationDto>();
    }
}

public sealed class CreateOrganizationCommandValidator
    : AbstractValidator<CreateOrganizationCommand>
{
    private static readonly string[] AllowedTypes =
    {
        "FARM", "PROCESSOR", "DISTRIBUTOR", "RETAILER", "INSPECTOR_ORG"
    };

    public CreateOrganizationCommandValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(t => AllowedTypes.Contains(t))
            .WithMessage("type must be one of: FARM, PROCESSOR, DISTRIBUTOR, RETAILER, INSPECTOR_ORG");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
