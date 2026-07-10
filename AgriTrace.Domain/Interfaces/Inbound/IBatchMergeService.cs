using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface IBatchMergeService
    : IService<BatchMerge, Guid>
{
    Task<IReadOnlyList<BatchMerge>> GetByNewBatchAsync(
        Guid newBatchId,
        CancellationToken cancellationToken = default);
}