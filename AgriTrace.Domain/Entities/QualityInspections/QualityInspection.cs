using AgriTrace.Domain.Common;
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

namespace AgriTrace.Domain.Entities.QualityInspections;

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

    /// <summary>
    /// Constructor for creating a new QualityInspection (generates a new Id).
    /// </summary>
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

    /// <summary>
    /// Rehydration constructor for reconstructing a QualityInspection from the database (preserves Id and audit fields).
    /// </summary>
    public QualityInspection(
        Guid id,
        Guid batchId,
        Guid inspectorId,
        InspectionStatus status,
        string? result,
        string? notes,
        DateTime createdAt,
        DateTime? updatedAt)
        : base(id, createdAt, updatedAt)
    {
        Validate(batchId, inspectorId);

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

