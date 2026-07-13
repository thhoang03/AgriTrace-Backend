using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities;

public class BatchMergeSource
{
    public Guid BatchMergeId { get; private set; }

    public Guid SourceBatchId { get; private set; }

    public decimal Quantity { get; private set; }

    public BatchMerge BatchMerge { get; private set; }

    public Batch SourceBatch { get; private set; }

    private BatchMergeSource()
    {

    }

    public BatchMergeSource(
        Guid batchMergeId,
        Guid sourceBatchId,
        decimal quantity)
    {
        BatchMergeId = batchMergeId;
        SourceBatchId = sourceBatchId;
        Quantity = quantity;
    }
}