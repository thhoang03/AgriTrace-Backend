using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface IBatchSplitRepository
    : IRepository<BatchSplit, Guid>
{
    Task<IReadOnlyList<BatchSplit>> GetBySourceBatchAsync(
        Guid sourceBatchId,
        CancellationToken cancellationToken = default);
}