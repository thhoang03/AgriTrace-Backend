using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Services;
using Mapster;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace AgriTrace.Application.Features.Organizations.Commands;

public sealed record UpdateOrganizationCommand(
    Guid Id,
    [Required] Guid OrganizationTypeId,
    [Required][StringLength(200, MinimumLength = 1)] string Name,
    [StringLength(500)] string? Address) : IRequest<OrganizationDto>;

public sealed class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, OrganizationDto>
{
    private readonly IOrganizationService _organizationService;
    private readonly IOrganizationTypeService _organizationTypeService;

    public UpdateOrganizationCommandHandler(IOrganizationService organizationService, IOrganizationTypeService organizationTypeService)
    {
        _organizationService = organizationService;
        _organizationTypeService = organizationTypeService;
    }

    public async Task<OrganizationDto> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var existing = await _organizationService.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Organization {request.Id} not found.");

        var duplicate = await _organizationService.GetByNameAsync(request.Name, cancellationToken);
        if (duplicate != null && duplicate.Id != request.Id)
            throw new ConflictException("Organization name already exists.");

        if (await _organizationTypeService.GetByIdAsync(request.OrganizationTypeId, cancellationToken) == null)
            throw new ConflictException("OrganizationType is not exist!");


        existing.UpdateInformation(request.OrganizationTypeId, request.Name, request.Address);
        await _organizationService.UpdateAsync(existing, cancellationToken);

        var dto = existing.Adapt<OrganizationDto>();
        var typeObj = await _organizationTypeService.GetByIdAsync(request.OrganizationTypeId, cancellationToken);
        if (typeObj != null)
        {
            dto.OrganizationTypeCode = typeObj.Code;
            dto.OrganizationTypeName = typeObj.Name;
        }
        return dto;
    }
}
