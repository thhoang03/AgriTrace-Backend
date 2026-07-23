using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Lookup.Queries;

public record GetUnitsQuery : IRequest<IReadOnlyList<LookupItemDto>>;

public class GetUnitsQueryHandler : IRequestHandler<GetUnitsQuery, IReadOnlyList<LookupItemDto>>
{
    private readonly IUnitService _unitService;

    public GetUnitsQueryHandler(IUnitService unitService)
    {
        _unitService = unitService;
    }

    public async Task<IReadOnlyList<LookupItemDto>> Handle(GetUnitsQuery request, CancellationToken cancellationToken)
    {
        var units = await _unitService.GetAllAsync(cancellationToken);

        return units
            .Select(u => new LookupItemDto
            {
                Id = u.Id.ToString(),
                Code = u.Code,
                Name = u.Name
            })
            .ToList();
    }
}
