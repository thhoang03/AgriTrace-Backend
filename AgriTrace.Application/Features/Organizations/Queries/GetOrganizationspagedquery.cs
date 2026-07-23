using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Common;
using AgriTrace.Domain.Interfaces.Inbound;
using Mapster;
using MediatR;

namespace AgriTrace.Application.Features.Organizations.Queries;

public record GetOrganizationsPagedQuery(
    int Page,
    int PageSize) : IRequest<PagedResult<OrganizationDto>>;

public class GetOrganizationsPagedQueryHandler : IRequestHandler<GetOrganizationsPagedQuery, PagedResult<OrganizationDto>>
{
    private readonly IOrganizationService _organizationService;

    public GetOrganizationsPagedQueryHandler(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    public async Task<PagedResult<OrganizationDto>> Handle(GetOrganizationsPagedQuery request, CancellationToken cancellationToken)
    {
        var pagedResult = await _organizationService.GetPagedAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtoItems = pagedResult.Items.Adapt<List<OrganizationDto>>();

        return new PagedResult<OrganizationDto>(
            dtoItems,
            pagedResult.TotalCount,
            pagedResult.PageNumber,
            pagedResult.PageSize);
    }
}