using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities;

public class BatchMerge : BaseEntity
{
    public Guid NewBatchId { get; private set; }

    public Batch NewBatch { get; private set; }

    private readonly List<BatchMergeSource> _sources = new();

    public IReadOnlyCollection<BatchMergeSource> Sources
        => _sources.AsReadOnly();

    private BatchMerge()
    {

    }

    public BatchMerge(Guid newBatchId)
    {
        if (newBatchId == Guid.Empty)
            throw new ArgumentException(nameof(newBatchId));

        NewBatchId = newBatchId;
    }
}