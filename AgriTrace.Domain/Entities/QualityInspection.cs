using AgriTrace.Domain.Common;
using AgriTrace.Domain.Common.Enums;

namespace AgriTrace.Domain.Entities;

public class QualityInspection : BaseEntity
{
    public Guid BatchId { get; private set; }

    public Guid InspectorId { get; private set; }

    public InspectionStatus Status { get; private set; }

    public string? Result { get; private set; }

    public string? Notes { get; private set; }

    // Navigation

    public Batch Batch { get; private set; }

    public User Inspector { get; private set; }

    private QualityInspection()
    {
    }

    public QualityInspection(
        Guid batchId,
        Guid inspectorId,
        InspectionStatus status,
        string? result,
        string? notes)
    {
        Validate(
            batchId,
            inspectorId);

        BatchId = batchId;
        InspectorId = inspectorId;
        Status = status;
        Result = result?.Trim();
        Notes = notes?.Trim();
    }

    public void UpdateResult(
        InspectionStatus status,
        string? result,
        string? notes)
    {
        Status = status;
        Result = result?.Trim();
        Notes = notes?.Trim();

        MarkUpdated();
    }

    private static void Validate(
        Guid batchId,
        Guid inspectorId)
    {
        if (batchId == Guid.Empty)
        {
            throw new ArgumentException("Batch is required.");
        }

        if (inspectorId == Guid.Empty)
        {
            throw new ArgumentException("Inspector is required.");
        }
    }
}