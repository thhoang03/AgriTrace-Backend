using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.Users;

namespace AgriTrace.Domain.Entities.QualityInspections;

public class QualityInspection : BaseEntity
{
    public Guid BatchId { get; private set; }

    public Guid? OrganizationId { get; private set; }

    public Guid? InspectorId { get; private set; }

    public InspectionType InspectionType { get; private set; }

    public InspectionStatus Status { get; private set; }

    public string? OverallResult { get; private set; }

    public DateTime InspectionDate { get; private set; }

    public string? Notes { get; private set; }

    // Navigation

    public Batch Batch { get; private set; }

    public User? Inspector { get; private set; }

    private readonly List<InspectionLabTest> _labTests = new();
    public IReadOnlyCollection<InspectionLabTest> LabTests => _labTests.AsReadOnly();

    private QualityInspection()
    {
    }

    /// <summary>
    /// Constructor for creating a new QualityInspection (generates a new Id).
    /// </summary>
    public QualityInspection(
        Guid batchId,
        Guid? organizationId,
        Guid? inspectorId,
        InspectionType inspectionType,
        DateTime inspectionDate,
        string? notes)
    {
        Validate(batchId, inspectorId);

        BatchId = batchId;
        OrganizationId = organizationId;
        InspectorId = inspectorId;
        InspectionType = inspectionType;
        Status = InspectionStatus.Pending;
        InspectionDate = inspectionDate;
        Notes = notes?.Trim();
    }

    /// <summary>
    /// Rehydration constructor for reconstructing a QualityInspection from the database (preserves Id and audit fields).
    /// </summary>
    public QualityInspection(
        Guid id,
        Guid batchId,
        Guid? organizationId,
        Guid? inspectorId,
        InspectionType inspectionType,
        InspectionStatus status,
        string? overallResult,
        DateTime inspectionDate,
        string? notes,
        DateTime createdAt,
        DateTime? updatedAt,
        IReadOnlyCollection<InspectionLabTest>? labTests = null,
        Batch? batch = null,
        User? inspector = null)
        : base(id, createdAt, updatedAt)
    {
        Validate(batchId, inspectorId);

        BatchId = batchId;
        OrganizationId = organizationId;
        InspectorId = inspectorId;
        InspectionType = inspectionType;
        Status = status;
        OverallResult = overallResult?.Trim();
        InspectionDate = inspectionDate;
        Notes = notes?.Trim();

        if (labTests != null)
        {
            _labTests.AddRange(labTests);
        }

        if (batch != null) Batch = batch;
        if (inspector != null) Inspector = inspector;
    }

    public void Conclude(string overallResult, string? notes)
    {
        if (overallResult != "PASS" && overallResult != "FAIL")
            throw new ArgumentException("OverallResult must be 'PASS' or 'FAIL'.");

        if (Status != InspectionStatus.Pending)
            throw new InvalidOperationException("Cannot conclude an inspection that is not in Pending status.");

        OverallResult = overallResult.Trim();
        Status = overallResult == "PASS"
            ? InspectionStatus.Passed
            : InspectionStatus.Failed;
        Notes = notes?.Trim();

        MarkUpdated();
    }

    public InspectionLabTest AddLabTest(
        string testName,
        string? measuredValue,
        string? unit,
        string? minStandardValue,
        string? maxStandardValue,
        bool isPassed,
        string? remark)
    {
        var test = new InspectionLabTest(
            Id,
            testName,
            measuredValue,
            unit,
            minStandardValue,
            maxStandardValue,
            isPassed,
            remark);

        _labTests.Add(test);
        MarkUpdated();

        return test;
    }

    private static void Validate(Guid batchId, Guid? inspectorId)
    {
        if (batchId == Guid.Empty)
            throw new ArgumentException("Batch is required.");
    }
}

