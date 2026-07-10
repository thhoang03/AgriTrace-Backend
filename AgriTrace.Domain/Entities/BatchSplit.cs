using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities;

public class BatchSplit : BaseEntity
{
    public Guid SourceBatchId { get; private set; }

    public Batch SourceBatch { get; private set; }

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
}