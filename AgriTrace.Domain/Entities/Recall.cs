using AgriTrace.Domain.Common;
using AgriTrace.Domain.Common.Enums;

namespace AgriTrace.Domain.Entities;

public class Recall : BaseEntity
{
    public Guid BatchId { get; private set; }

    public Guid CreatedBy { get; private set; }

    public string Reason { get; private set; }

    public RecallSeverity Severity { get; private set; }

    public RecallStatus Status { get; private set; }

    // Navigation

    public Batch Batch { get; private set; }

    public User CreatedUser { get; private set; }

    private Recall()
    {
    }

    public Recall(
        Guid batchId,
        Guid createdBy,
        string reason,
        RecallSeverity severity)
    {
        Validate(
            batchId,
            createdBy,
            reason);

        BatchId = batchId;
        CreatedBy = createdBy;
        Reason = reason.Trim();
        Severity = severity;
        Status = RecallStatus.Pending;
    }

    /// <summary>
    /// Rehydrates a Recall from persisted storage. Infrastructure use only.
    /// </summary>
    public static Recall Rehydrate(
        Guid id,
        Guid batchId,
        Guid createdBy,
        string reason,
        RecallSeverity severity,
        RecallStatus status,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        var recall = new Recall
        {
            Id = id,
            BatchId = batchId,
            CreatedBy = createdBy,
            Reason = reason,
            Severity = severity,
            Status = status,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        return recall;
    }

    public void StartProcessing()
    {
        Status = RecallStatus.Processing;
        MarkUpdated();
    }

    public void Complete()
    {
        Status = RecallStatus.Completed;
        MarkUpdated();
    }

    public void Cancel()
    {
        Status = RecallStatus.Cancelled;
        MarkUpdated();
    }

    public void UpdateReason(
        string reason,
        RecallSeverity severity)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Recall reason is required.");
        }

        Reason = reason.Trim();
        Severity = severity;

        MarkUpdated();
    }

    private static void Validate(
        Guid batchId,
        Guid createdBy,
        string reason)
    {
        if (batchId == Guid.Empty)
        {
            throw new ArgumentException("Batch is required.");
        }

        if (createdBy == Guid.Empty)
        {
            throw new ArgumentException("CreatedBy is required.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Recall reason is required.");
        }
    }
}