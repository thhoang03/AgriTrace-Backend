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
