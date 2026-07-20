using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Events.Queries;

public record GetEventByIdQuery(
    Guid EventId) : IRequest<EventDto>;

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDto>
{
    private readonly IEventService _eventService;
    private readonly IEventTypeService _eventTypeService;

    public GetEventByIdQueryHandler(
        IEventService eventService,
        IEventTypeService eventTypeService)
    {
        _eventService = eventService;
        _eventTypeService = eventTypeService;
    }

    public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var e = await _eventService.GetByIdAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException($"Event {request.EventId} not found.");

        var type = await _eventTypeService.GetByIdAsync(e.EventTypeId, cancellationToken);

        return EventMapper.ToDto(e, type?.Code);
    }
}
