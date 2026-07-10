using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities;

public class BatchSplitDetail : BaseEntity
{
    public Guid SplitId { get; private set; }

    public Guid TargetBatchId { get; private set; }

    public decimal Quantity { get; private set; }

    public BatchSplit Split { get; private set; }

    public Batch TargetBatch { get; private set; }

    private BatchSplitDetail()
    {

    }

    public BatchSplitDetail(
        Guid splitId,
        Guid targetBatchId,
        decimal quantity)
    {
        SplitId = splitId;
        TargetBatchId = targetBatchId;
        Quantity = quantity;
    }
}