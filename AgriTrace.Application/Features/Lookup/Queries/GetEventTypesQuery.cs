using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Lookup.Queries;

public record GetEventTypesQuery : IRequest<IReadOnlyList<LookupItemDto>>;

public class GetEventTypesQueryHandler : IRequestHandler<GetEventTypesQuery, IReadOnlyList<LookupItemDto>>
{
    private readonly IEventTypeService _eventTypeService;

    public GetEventTypesQueryHandler(IEventTypeService eventTypeService)
    {
        _eventTypeService = eventTypeService;
    }

    public async Task<IReadOnlyList<LookupItemDto>> Handle(GetEventTypesQuery request, CancellationToken cancellationToken)
    {
        var types = await _eventTypeService.GetAllAsync(cancellationToken);

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
