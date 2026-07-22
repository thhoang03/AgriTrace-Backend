using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Outbound;

public interface IBatchSplitRepository
    : IRepository<BatchSplit, Guid>
{
    Task<IReadOnlyList<BatchSplit>> GetBySourceBatchAsync(
        Guid sourceBatchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Persist toàn bộ kết quả 1 lần Split trong 1 transaction:
    /// batch gốc (update RemainingQuantity), batch con (insert),
    /// BatchSplit + BatchSplitDetail (insert), SupplyChainEvent (insert).
    /// </summary>
    Task SaveSplitAsync(
        Batch sourceBatch,
        IReadOnlyList<Batch> childBatches,
        BatchSplit batchSplit,
        IReadOnlyList<BatchSplitDetail> details,
        SupplyChainEvent splitEvent,
        CancellationToken cancellationToken = default);
}