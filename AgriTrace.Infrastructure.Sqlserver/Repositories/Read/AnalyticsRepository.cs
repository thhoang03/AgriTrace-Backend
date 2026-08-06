using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Domain.Models.Analytics;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgriTrace.Infrastructure.Sqlserver.Repositories.Read;

public class AnalyticsRepository : IAnalyticsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AnalyticsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private IQueryable<BatchDataModel> GetOrgBatchesQuery(Guid? organizationId)
    {
        var query = _dbContext.Batches.AsNoTracking();
        if (organizationId.HasValue)
        {
            query = query.Where(b => b.CurrentOrganizationId == organizationId.Value || b.Events.Any(e => e.OrganizationId == organizationId.Value));
        }
        return query;
    }

    public async Task<OverviewResult> GetOverviewAsync(Guid? organizationId, CancellationToken cancellationToken = default)
    {
        var orgBatchesQuery = GetOrgBatchesQuery(organizationId);

        var totalBatches = await orgBatchesQuery.CountAsync(cancellationToken);
        
        var recalledBatches = await orgBatchesQuery.CountAsync(b => b.Status == BatchStatus.Recalled, cancellationToken);
        var activeBatches = totalBatches - recalledBatches;

        var totalOrganizationsQuery = _dbContext.Organizations.AsNoTracking();
        if (organizationId.HasValue)
        {
            // If scoped to org, maybe they only want to see 1? Or total orgs they interacted with?
            // "total orgs they interacted with" = unique orgs across events of their batches.
            var interactingOrgs = await orgBatchesQuery
                .SelectMany(b => b.Events.Select(e => e.OrganizationId))
                .Distinct()
                .ToListAsync(cancellationToken);
            
            var currentOrgs = await orgBatchesQuery
                .Select(b => b.CurrentOrganizationId)
                .Distinct()
                .ToListAsync(cancellationToken);
                
            var allOrgIds = interactingOrgs.Concat(currentOrgs).Distinct().ToList();
            if (!allOrgIds.Contains(organizationId.Value)) allOrgIds.Add(organizationId.Value);
            
            totalOrganizationsQuery = totalOrganizationsQuery.Where(o => allOrgIds.Contains(o.Id));
        }
        var totalOrganizations = await totalOrganizationsQuery.CountAsync(cancellationToken);

        var totalEvents = await orgBatchesQuery.SelectMany(b => b.Events).CountAsync(cancellationToken);
        var totalRecalls = await orgBatchesQuery.SelectMany(b => b.Recalls).CountAsync(cancellationToken);

        var monthlyProduction = await orgBatchesQuery
            .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Quantity = g.Sum(b => b.Quantity),
                Batches = g.Count()
            })
            .ToListAsync(cancellationToken);

        var batchStatus = await orgBatchesQuery
            .GroupBy(b => b.Status)
            .Select(g => new
            {
                Status = (int)g.Key,
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        var inspections = await orgBatchesQuery
            .SelectMany(b => b.QualityInspections)
            .GroupBy(i => new { i.InspectionDate.Year, i.InspectionDate.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Pass = g.Count(i => i.Status == InspectionStatus.Passed),
                Fail = g.Count(i => i.Status == InspectionStatus.Failed),
                Pending = g.Count(i => i.Status == InspectionStatus.Pending)
            })
            .ToListAsync(cancellationToken);

        var recalls = await orgBatchesQuery
            .SelectMany(b => b.Recalls)
            .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Recalls = g.Count()
            })
            .ToListAsync(cancellationToken);

        return new OverviewResult
        {
            TotalBatches = totalBatches,
            TotalOrganizations = totalOrganizations,
            TotalEvents = totalEvents,
            TotalRecalls = totalRecalls,
            ActiveBatches = activeBatches,
            RecalledBatches = recalledBatches,
            MonthlyProduction = monthlyProduction.Select(m => new MonthlyProductionResult
            {
                Month = new DateTime(m.Year, m.Month, 1).ToString("MMM"),
                Quantity = m.Quantity,
                Batches = m.Batches
            }).OrderBy(m => m.Month).ToList(),
            BatchStatus = batchStatus.Select(b => new BatchStatusDistributionResult
            {
                Status = b.Status,
                StatusName = ((BatchStatus)b.Status).ToString().ToUpperInvariant(),
                Count = b.Count
            }).OrderBy(b => b.Status).ToList(),
            InspectionResults = inspections.Select(i => new InspectionResultData
            {
                Month = new DateTime(i.Year, i.Month, 1).ToString("MMM"),
                Pass = i.Pass,
                Fail = i.Fail,
                Pending = i.Pending
            }).OrderBy(i => i.Month).ToList(),
            RecallTrend = recalls.Select(r => new RecallTrendResult
            {
                Month = new DateTime(r.Year, r.Month, 1).ToString("MMM"),
                Recalls = r.Recalls
            }).OrderBy(r => r.Month).ToList()
        };
    }

    public async Task<BatchDistributionResult> GetBatchDistributionAsync(Guid? organizationId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var query = GetOrgBatchesQuery(organizationId);
        
        if (fromDate.HasValue) query = query.Where(b => b.CreatedAt >= fromDate.Value);
        if (toDate.HasValue) query = query.Where(b => b.CreatedAt <= toDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .GroupBy(b => b.Status)
            .Select(g => new
            {
                Status = (int)g.Key,
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        return new BatchDistributionResult
        {
            TotalCount = totalCount,
            Items = items.Select(i => new BatchStatusDistributionResult
            {
                Status = i.Status,
                StatusName = ((BatchStatus)i.Status).ToString().ToUpperInvariant(),
                Count = i.Count
            }).OrderBy(i => i.Status).ToList()
        };
    }

    public async Task<ProcessingTimeResult> GetProcessingTimeAsync(Guid? organizationId, Guid? eventTypeId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var query = GetOrgBatchesQuery(organizationId);

        // Filter events within the date range and fetch only needed fields
        var batchesWithEvents = await query
            .Select(b => new
            {
                b.Id,
                Events = b.Events
                    .Where(e => (!fromDate.HasValue || e.EventTime >= fromDate.Value) && 
                                (!toDate.HasValue || e.EventTime <= toDate.Value))
                    .OrderBy(e => e.EventTime)
                    .Select(e => new { e.EventTypeId, e.EventTime, EventTypeCode = e.EventType.Code })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        var hoursByType = new Dictionary<Guid, (string Code, List<double> Gaps)>();
        var allGaps = new List<double>();

        foreach (var batch in batchesWithEvents)
        {
            for (var i = 1; i < batch.Events.Count; i++)
            {
                var gapHours = (batch.Events[i].EventTime - batch.Events[i - 1].EventTime).TotalHours;
                if (gapHours < 0) continue;

                var currentType = batch.Events[i];
                if (eventTypeId.HasValue && currentType.EventTypeId != eventTypeId.Value) continue;

                if (!hoursByType.TryGetValue(currentType.EventTypeId, out var entry))
                {
                    entry = (currentType.EventTypeCode, new List<double>());
                    hoursByType[currentType.EventTypeId] = entry;
                }

                entry.Gaps.Add(gapHours);
                allGaps.Add(gapHours);
            }
        }

        var byEventType = hoursByType.Select(kvp => new ProcessingTimeByEventTypeResult
        {
            EventTypeCode = kvp.Value.Code,
            AverageHours = Math.Round(kvp.Value.Gaps.Average(), 2)
        }).OrderBy(x => x.EventTypeCode).ToList();

        return new ProcessingTimeResult
        {
            AverageProcessingHours = allGaps.Count > 0 ? Math.Round(allGaps.Average(), 2) : 0,
            ByEventType = byEventType
        };
    }
}
