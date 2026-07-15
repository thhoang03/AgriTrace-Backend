using System;
using System.Threading;
using System.Threading.Tasks;
using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Organizations.Queries;

public record GetOrganizationByIdQuery(Guid Id) : IRequest<OrganizationDto>;

public class GetOrganizationByIdQueryHandler : IRequestHandler<GetOrganizationByIdQuery, OrganizationDto>
{
    private readonly IOrganizationService _organizationService;

    public GetOrganizationByIdQueryHandler(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    public async Task<OrganizationDto> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
    {
        var organization = await _organizationService.GetByIdAsync(request.Id, cancellationToken);

        if (organization is null)
            throw new NotFoundException($"Organization with Id '{request.Id}' was not found.");

        return organization.Adapt<OrganizationDto>();
    }
}