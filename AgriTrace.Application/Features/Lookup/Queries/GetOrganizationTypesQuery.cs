using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Lookup.Queries;

public record GetOrganizationTypesQuery : IRequest<IReadOnlyList<LookupItemDto>>;

public class GetOrganizationTypesQueryHandler : IRequestHandler<GetOrganizationTypesQuery, IReadOnlyList<LookupItemDto>>
{
    private readonly IOrganizationTypeService _organizationTypeService;

    public GetOrganizationTypesQueryHandler(IOrganizationTypeService organizationTypeService)
    {
        _organizationTypeService = organizationTypeService;
    }

    public async Task<IReadOnlyList<LookupItemDto>> Handle(GetOrganizationTypesQuery request, CancellationToken cancellationToken)
    {
        var types = await _organizationTypeService.GetAllAsync(cancellationToken);

        return types
            .Select(t => new LookupItemDto
            {
                Id = t.Id.ToString(),
                Code = t.Code,
                Name = t.Name
            })
            .ToList();
    }
}
