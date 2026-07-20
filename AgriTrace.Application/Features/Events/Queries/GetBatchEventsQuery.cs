using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Common;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Events.Queries;

public record GetBatchEventsQuery(
    Guid BatchId,
    int Page,
    int PageSize) : IRequest<PagedResult<EventDto>>;

public class GetBatchEventsQueryHandler : IRequestHandler<GetBatchEventsQuery, PagedResult<EventDto>>
{
    private readonly IEventService _eventService;
    private readonly IEventTypeService _eventTypeService;

    public GetBatchEventsQueryHandler(
        IEventService eventService,
        IEventTypeService eventTypeService)
    {
        _eventService = eventService;
        _eventTypeService = eventTypeService;
    }

    public async Task<PagedResult<EventDto>> Handle(GetBatchEventsQuery request, CancellationToken cancellationToken)
    {
        var events = await _eventService.GetByBatchAsync(request.BatchId, cancellationToken);

        var totalCount = events.Count;

        var pageItems = events
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Resolve event type codes once per distinct type.
        var typeCodes = new Dictionary<Guid, string?>();
        foreach (var typeId in pageItems.Select(e => e.EventTypeId).Distinct())
        {
            var type = await _eventTypeService.GetByIdAsync(typeId, cancellationToken);
            typeCodes[typeId] = type?.Code;
        }

        var items = pageItems
            .Select(e => EventMapper.ToDto(e, typeCodes.GetValueOrDefault(e.EventTypeId)))
            .ToList();

        return new PagedResult<EventDto>(items, totalCount, request.Page, request.PageSize);
    }
}
