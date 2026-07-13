using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface IBatchMergeRepository
    : IRepository<BatchMerge, Guid>
{
    Task<IReadOnlyList<BatchMerge>> GetByNewBatchAsync(
        Guid newBatchId,
        CancellationToken cancellationToken = default);
}