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
    private readonly IQualityInspectionService _qualityInspectionService;

    public GetOverviewQueryHandler(
        IBatchReadService batchReadService,
        IOrganizationService organizationService,
        IEventService eventService,
        IRecallService recallService,
        IQualityInspectionService qualityInspectionService)
    {
        _batchReadService = batchReadService;
        _organizationService = organizationService;
        _eventService = eventService;
        _recallService = recallService;
        _qualityInspectionService = qualityInspectionService;
    }

    public async Task<OverviewDto> Handle(GetOverviewQuery request, CancellationToken cancellationToken)
    {
        var batches = await _batchReadService.GetAllAsync(cancellationToken);
        var organizations = await _organizationService.GetAllAsync(cancellationToken);
        var recalls = await _recallService.GetAllAsync(cancellationToken);
        var inspections = await _qualityInspectionService.GetAllAsync(cancellationToken);

        var totalEvents = 0;
        foreach (var batch in batches)
        {
            var events = await _eventService.GetByBatchAsync(batch.Id, cancellationToken);
            totalEvents += events.Count;
        }

        var recalledBatches = batches.Count(b => b.Status == BatchStatus.Recalled);

        var monthlyProduction = batches
            .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new MonthlyProductionDto
            {
                Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                Quantity = g.Sum(b => b.Quantity),
                Batches = g.Count()
            })
            .ToList();

        var batchStatus = batches
            .GroupBy(b => b.Status)
            .Select(g => new BatchStatusDistributionItemDto
            {
                Status = (int)g.Key,
                StatusName = g.Key.ToString().ToUpperInvariant(),
                Count = g.Count()
            })
            .OrderBy(i => i.Status)
            .ToList();

        var inspectionResults = inspections
            .GroupBy(i => new { i.InspectionDate.Year, i.InspectionDate.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new InspectionResultDto
            {
                Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                Pass = g.Count(i => i.Status == InspectionStatus.Passed),
                Fail = g.Count(i => i.Status == InspectionStatus.Failed),
                Pending = g.Count(i => i.Status == InspectionStatus.Pending)
            })
            .ToList();

        var recallTrend = recalls
            .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new RecallTrendDto
            {
                Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                Recalls = g.Count()
            })
            .ToList();

        return new OverviewDto
        {
            TotalBatches = batches.Count,
            TotalOrganizations = organizations.Count,
            TotalEvents = totalEvents,
            TotalRecalls = recalls.Count,
            ActiveBatches = batches.Count - recalledBatches,
            RecalledBatches = recalledBatches,
            MonthlyProduction = monthlyProduction,
            BatchStatus = batchStatus,
            InspectionResults = inspectionResults,
            RecallTrend = recallTrend
        };
    }
}

