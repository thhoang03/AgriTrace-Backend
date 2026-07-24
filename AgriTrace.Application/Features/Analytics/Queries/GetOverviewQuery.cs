using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.Categories;
using AgriTrace.Domain.Entities.Certificates;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.Notifications;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Entities.Recalls;
using AgriTrace.Domain.Entities.Units;
using AgriTrace.Domain.Entities.Users;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Analytics.Queries;

public record GetOverviewQuery : IRequest<OverviewDto>;

public class GetOverviewQueryHandler : IRequestHandler<GetOverviewQuery, OverviewDto>
{
    private readonly IBatchReadService _batchReadService;
    private readonly IOrganizationService _organizationService;
    private readonly IEventService _eventService;
    private readonly IRecallService _recallService;

    public GetOverviewQueryHandler(
        IBatchReadService batchReadService,
        IOrganizationService organizationService,
        IEventService eventService,
        IRecallService recallService)
    {
        _batchReadService = batchReadService;
        _organizationService = organizationService;
        _eventService = eventService;
        _recallService = recallService;
    }

    public async Task<OverviewDto> Handle(GetOverviewQuery request, CancellationToken cancellationToken)
    {
        var batches = await _batchReadService.GetAllAsync(cancellationToken);
        var organizations = await _organizationService.GetAllAsync(cancellationToken);
        var recalls = await _recallService.GetAllAsync(cancellationToken);

        // IEventService exposes only per-batch access; aggregate the count across batches.
        var totalEvents = 0;
        foreach (var batch in batches)
        {
            var events = await _eventService.GetByBatchAsync(batch.Id, cancellationToken);
            totalEvents += events.Count;
        }

        var recalledBatches = batches.Count(b => b.Status == BatchStatus.Recalled);

        return new OverviewDto
        {
            TotalBatches = batches.Count,
            TotalOrganizations = organizations.Count,
            TotalEvents = totalEvents,
            TotalRecalls = recalls.Count,
            ActiveBatches = batches.Count - recalledBatches,
            RecalledBatches = recalledBatches
        };
    }
}

