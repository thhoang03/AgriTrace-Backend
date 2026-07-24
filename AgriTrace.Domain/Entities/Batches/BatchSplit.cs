using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities.Batches;

public class BatchSplit : BaseEntity
{
    public Guid SourceBatchId { get; private set; }

    public Batch SourceBatch { get; private set; } = null!;

    private readonly List<BatchSplitDetail> _details = new();

    public IReadOnlyCollection<BatchSplitDetail> Details
        => _details.AsReadOnly();

    private BatchSplit()
    {

    }

    public BatchSplit(Guid sourceBatchId)
    {
        if (sourceBatchId == Guid.Empty)
            throw new ArgumentException(nameof(sourceBatchId));

        SourceBatchId = sourceBatchId;
    }

    public BatchSplitDetail AddDetail(Guid targetBatchId, decimal quantity)
    {
        var detail = new BatchSplitDetail(Id, targetBatchId, quantity);
        _details.Add(detail);
        return detail;
    }
}
