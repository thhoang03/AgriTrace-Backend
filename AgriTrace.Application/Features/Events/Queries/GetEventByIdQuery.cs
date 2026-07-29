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
    private readonly IOrganizationService _organizationService;
    private readonly IUserService _userService;

    public GetEventByIdQueryHandler(
        IEventService eventService,
        IEventTypeService eventTypeService,
        IOrganizationService organizationService,
        IUserService userService)
    {
        _eventService = eventService;
        _eventTypeService = eventTypeService;
        _organizationService = organizationService;
        _userService = userService;
    }

    public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var e = await _eventService.GetByIdAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException($"Event {request.EventId} not found.");

        var type = await _eventTypeService.GetByIdAsync(e.EventTypeId, cancellationToken);

        var org = await _organizationService.GetByIdAsync(e.OrganizationId, cancellationToken);
        var user = await _userService.GetByIdAsync(e.PerformedByUserId, cancellationToken);

        return EventMapper.ToDto(e, type?.Code, org?.Name, user?.FullName ?? user?.Email);
    }
}
