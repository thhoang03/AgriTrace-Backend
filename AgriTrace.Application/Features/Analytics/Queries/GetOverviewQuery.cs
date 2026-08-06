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
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Domain.Models.Analytics;
using MediatR;

namespace AgriTrace.Application.Features.Analytics.Queries;

public record GetOverviewQuery(Guid? OrganizationId = null) : IRequest<OverviewDto>;

public class GetOverviewQueryHandler : IRequestHandler<GetOverviewQuery, OverviewDto>
{
    private readonly IAnalyticsRepository _analyticsRepository;

    public GetOverviewQueryHandler(IAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    public async Task<OverviewDto> Handle(GetOverviewQuery request, CancellationToken cancellationToken)
    {
        var result = await _analyticsRepository.GetOverviewAsync(request.OrganizationId, cancellationToken);

        return new OverviewDto
        {
            TotalBatches = result.TotalBatches,
            TotalOrganizations = result.TotalOrganizations,
            TotalEvents = result.TotalEvents,
            TotalRecalls = result.TotalRecalls,
            ActiveBatches = result.ActiveBatches,
            RecalledBatches = result.RecalledBatches,
            MonthlyProduction = result.MonthlyProduction.Select(m => new MonthlyProductionDto
            {
                Month = m.Month,
                Quantity = m.Quantity,
                Batches = m.Batches
            }).ToList(),
            BatchStatus = result.BatchStatus.Select(b => new BatchStatusDistributionItemDto
            {
                Status = b.Status,
                StatusName = b.StatusName,
                Count = b.Count
            }).ToList(),
            InspectionResults = result.InspectionResults.Select(i => new InspectionResultDto
            {
                Month = i.Month,
                Pass = i.Pass,
                Fail = i.Fail,
                Pending = i.Pending
            }).ToList(),
            RecallTrend = result.RecallTrend.Select(r => new RecallTrendDto
            {
                Month = r.Month,
                Recalls = r.Recalls
            }).ToList()
        };
    }
}

