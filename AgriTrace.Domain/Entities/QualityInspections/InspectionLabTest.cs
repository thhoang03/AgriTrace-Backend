using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities.QualityInspections;

public class InspectionLabTest : BaseEntity
{
    public Guid InspectionId { get; private set; }

    public string TestName { get; private set; }

    public string? MeasuredValue { get; private set; }

    public string? Unit { get; private set; }

    public string? MinStandardValue { get; private set; }

    public string? MaxStandardValue { get; private set; }

    public bool IsPassed { get; private set; }

    public string? Remark { get; private set; }

    public QualityInspection Inspection { get; private set; }

    private InspectionLabTest()
    {
    }

    public InspectionLabTest(
        Guid inspectionId,
        string testName,
        string? measuredValue,
        string? unit,
        string? minStandardValue,
        string? maxStandardValue,
        bool isPassed,
        string? remark)
    {
        Validate(inspectionId, testName);

        InspectionId = inspectionId;
        TestName = testName.Trim();
        MeasuredValue = measuredValue?.Trim();
        Unit = unit?.Trim();
        MinStandardValue = minStandardValue?.Trim();
        MaxStandardValue = maxStandardValue?.Trim();
        IsPassed = isPassed;
        Remark = remark?.Trim();
    }

    public InspectionLabTest(
        Guid id,
        Guid inspectionId,
        string testName,
        string? measuredValue,
        string? unit,
        string? minStandardValue,
        string? maxStandardValue,
        bool isPassed,
        string? remark,
        DateTime createdAt,
        DateTime? updatedAt)
        : base(id, createdAt, updatedAt)
    {
        Validate(inspectionId, testName);

        InspectionId = inspectionId;
        TestName = testName.Trim();
        MeasuredValue = measuredValue?.Trim();
        Unit = unit?.Trim();
        MinStandardValue = minStandardValue?.Trim();
        MaxStandardValue = maxStandardValue?.Trim();
        IsPassed = isPassed;
        Remark = remark?.Trim();
    }

    public void Update(
        string testName,
        string? measuredValue,
        string? unit,
        string? minStandardValue,
        string? maxStandardValue,
        bool isPassed,
        string? remark)
    {
        Validate(InspectionId, testName);

        TestName = testName.Trim();
        MeasuredValue = measuredValue?.Trim();
        Unit = unit?.Trim();
        MinStandardValue = minStandardValue?.Trim();
        MaxStandardValue = maxStandardValue?.Trim();
        IsPassed = isPassed;
        Remark = remark?.Trim();

        MarkUpdated();
    }

    private static void Validate(Guid inspectionId, string testName)
    {
        if (inspectionId == Guid.Empty)
            throw new ArgumentException("InspectionId is required.");

        if (string.IsNullOrWhiteSpace(testName))
            throw new ArgumentException("Test name is required.");
    }
}
