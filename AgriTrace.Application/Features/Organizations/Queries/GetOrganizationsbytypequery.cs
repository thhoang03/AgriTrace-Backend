using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Organizations.Queries;

public record GetOrganizationsByTypeQuery(Guid OrganizationTypeId) : IRequest<IReadOnlyList<OrganizationDto>>;

public class GetOrganizationsByTypeQueryHandler : IRequestHandler<GetOrganizationsByTypeQuery, IReadOnlyList<OrganizationDto>>
{
    private readonly IOrganizationService _organizationService;

    public GetOrganizationsByTypeQueryHandler(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    public async Task<IReadOnlyList<OrganizationDto>> Handle(GetOrganizationsByTypeQuery request, CancellationToken cancellationToken)
    {
        var organizations = await _organizationService.GetByTypeAsync(
            request.OrganizationTypeId,
            cancellationToken);

        return organizations.Adapt<List<OrganizationDto>>();
    }
}