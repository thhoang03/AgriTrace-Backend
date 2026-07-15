using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Organizations.Commands;

public record UpdateOrganizationStatusCommand(
    Guid Id,
    bool IsActive) : IRequest<OrganizationDto>;

public class UpdateOrganizationStatusCommandHandler : IRequestHandler<UpdateOrganizationStatusCommand, OrganizationDto>
{
    private readonly IOrganizationService _organizationService;

    public UpdateOrganizationStatusCommandHandler(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    public async Task<OrganizationDto> Handle(UpdateOrganizationStatusCommand request, CancellationToken cancellationToken)
    {
        var organization = await _organizationService.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Organization {request.Id} not found.");

        if (request.IsActive)
            organization.Activate();
        else
            organization.Deactivate();

        await _organizationService.UpdateAsync(organization, cancellationToken);

        return organization.Adapt<OrganizationDto>();
    }
}
