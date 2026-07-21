using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Analytics.Queries;

public record GetProcessingTimeQuery(
    Guid? OrganizationId,
    Guid? EventTypeId,
    DateTime? FromDate,
    DateTime? ToDate) : IRequest<ProcessingTimeDto>;

public class GetProcessingTimeQueryHandler : IRequestHandler<GetProcessingTimeQuery, ProcessingTimeDto>
{
    private readonly IBatchReadService _batchReadService;
    private readonly IEventService _eventService;
    private readonly IEventTypeService _eventTypeService;

    public GetProcessingTimeQueryHandler(
        IBatchReadService batchReadService,
        IEventService eventService,
        IEventTypeService eventTypeService)
    {
        _batchReadService = batchReadService;
        _eventService = eventService;
        _eventTypeService = eventTypeService;
    }

    public async Task<ProcessingTimeDto> Handle(GetProcessingTimeQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Batch> batches = request.OrganizationId.HasValue
            ? await _batchReadService.GetByOrganizationAsync(request.OrganizationId.Value, cancellationToken)
            : await _batchReadService.GetAllAsync(cancellationToken);

        // Accumulate elapsed hours attributed to each event type (gap from the previous event).
        var hoursByType = new Dictionary<Guid, List<double>>();
        var allGaps = new List<double>();

        foreach (var batch in batches)
        {
            var events = (await _eventService.GetByBatchAsync(batch.Id, cancellationToken))
                .Where(e =>
                    (!request.FromDate.HasValue || e.EventTime >= request.FromDate.Value) &&
                    (!request.ToDate.HasValue || e.EventTime <= request.ToDate.Value))
                .OrderBy(e => e.EventTime)
                .ToList();

            for (var i = 1; i < events.Count; i++)
            {
                var gapHours = (events[i].EventTime - events[i - 1].EventTime).TotalHours;
                if (gapHours < 0)
                {
                    continue;
                }

                var typeId = events[i].EventTypeId;
                if (request.EventTypeId.HasValue && typeId != request.EventTypeId.Value)
                {
                    continue;
                }

                if (!hoursByType.TryGetValue(typeId, out var list))
                {
                    list = new List<double>();
                    hoursByType[typeId] = list;
                }

                list.Add(gapHours);
                allGaps.Add(gapHours);
            }
        }

        var byEventType = new List<ProcessingTimeByEventTypeDto>();
        foreach (var kvp in hoursByType)
        {
            var type = await _eventTypeService.GetByIdAsync(kvp.Key, cancellationToken);
            byEventType.Add(new ProcessingTimeByEventTypeDto
            {
                EventTypeCode = type?.Code,
                AverageHours = Math.Round(kvp.Value.Average(), 2)
            });
        }

        return new ProcessingTimeDto
        {
            AverageProcessingHours = allGaps.Count > 0 ? Math.Round(allGaps.Average(), 2) : 0,
            ByEventType = byEventType.OrderBy(x => x.EventTypeCode).ToList()
        };
    }
}
