using AgriTrace.Domain.Models.Analytics;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface IAnalyticsRepository
{
    Task<OverviewResult> GetOverviewAsync(Guid? organizationId, CancellationToken cancellationToken = default);
    
    Task<BatchDistributionResult> GetBatchDistributionAsync(Guid? organizationId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
    
    Task<ProcessingTimeResult> GetProcessingTimeAsync(Guid? organizationId, Guid? eventTypeId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
}
