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
    private readonly IOrganizationService _organizationService;
    private readonly IUserService _userService;

    public GetBatchEventsQueryHandler(
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

        // Resolve organization and user names.
        var orgIds = pageItems.Select(e => e.OrganizationId).Distinct().ToList();
        var userIds = pageItems.Select(e => e.PerformedByUserId).Distinct().ToList();
        var orgNames = new Dictionary<Guid, string?>();
        var userNames = new Dictionary<Guid, string?>();
        foreach (var oid in orgIds)
        {
            var org = await _organizationService.GetByIdAsync(oid, cancellationToken);
            orgNames[oid] = org?.Name;
        }
        foreach (var uid in userIds)
        {
            var u = await _userService.GetByIdAsync(uid, cancellationToken);
            userNames[uid] = u?.FullName ?? u?.Email;
        }

        var items = pageItems
            .Select(e => EventMapper.ToDto(e, typeCodes.GetValueOrDefault(e.EventTypeId),
                orgNames.GetValueOrDefault(e.OrganizationId),
                userNames.GetValueOrDefault(e.PerformedByUserId)))
            .ToList();

        return new PagedResult<EventDto>(items, totalCount, request.Page, request.PageSize);
    }
}
