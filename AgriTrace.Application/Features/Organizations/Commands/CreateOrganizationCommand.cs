using System.ComponentModel.DataAnnotations;
using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using FluentValidation;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Organizations.Commands;

public record CreateOrganizationCommand(
    [Required] Guid OrganizationTypeId,
    [Required][StringLength(200, MinimumLength = 1)] string Name,
    [StringLength(500)] string? Address) : IRequest<OrganizationDto>;

public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, OrganizationDto>
{
    private readonly IOrganizationService _organizationService;
    private readonly IOrganizationTypeService _organizationTypeService;

    public CreateOrganizationCommandHandler(IOrganizationService organizationService,IOrganizationTypeService organizationTypeService)
    {
        _organizationService = organizationService;
        _organizationTypeService = organizationTypeService;
    }

    public async Task<OrganizationDto> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        if (await _organizationService.GetByNameAsync(request.Name, cancellationToken) != null)
            throw new ConflictException("Organization name already exists.");
        if (await _organizationTypeService.GetByIdAsync(request.OrganizationTypeId, cancellationToken) == null)
            throw new ConflictException("OrganizationType is not exist!");

        var organization = new Organization(request.OrganizationTypeId, request.Name, request.Address);
        var created = await _organizationService.CreateAsync(organization, cancellationToken);

        return created.Adapt<OrganizationDto>();
    }
}

public sealed class CreateOrganizationCommandValidator
    : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
