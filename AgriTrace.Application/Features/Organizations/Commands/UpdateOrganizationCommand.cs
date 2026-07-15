using System.ComponentModel.DataAnnotations;
using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Organizations.Commands;

public sealed record UpdateOrganizationCommand(
    Guid Id,
    [Required] Guid OrganizationTypeId,
    [Required][StringLength(200, MinimumLength = 1)] string Name,
    [StringLength(500)] string? Address) : IRequest<OrganizationDto>;

public sealed class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, OrganizationDto>
{
    private readonly IOrganizationService _organizationService;

    public UpdateOrganizationCommandHandler(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    public async Task<OrganizationDto> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var existing = await _organizationService.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Organization {request.Id} not found.");

        var duplicate = await _organizationService.GetByNameAsync(request.Name, cancellationToken);
        if (duplicate != null && duplicate.Id != request.Id)
            throw new ConflictException("Organization name already exists.");

        existing.UpdateInformation(request.OrganizationTypeId, request.Name, request.Address);
        await _organizationService.UpdateAsync(existing, cancellationToken);

        return existing.Adapt<OrganizationDto>();
    }
}
