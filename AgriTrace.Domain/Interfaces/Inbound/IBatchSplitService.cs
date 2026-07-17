using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Inbound;

public interface IBatchSplitService
    : IService<BatchSplit, Guid>
{
    Task<IReadOnlyList<BatchSplit>> GetBySourceBatchAsync(
        Guid sourceBatchId,
        CancellationToken cancellationToken = default);

    Task<BatchSplit> SplitBatchAsync(
        Guid sourceBatchId,
        Guid performedByUserId,
        IReadOnlyList<SplitPartRequest> parts,
        CancellationToken cancellationToken = default);
}
public record SplitPartRequest(string BatchCode, decimal Quantity);