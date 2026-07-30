namespace AgriTrace.Application.Contracts;

public sealed class InspectionLabTestDto
{
    public Guid Id { get; init; }
    public Guid InspectionId { get; init; }
    public string TestName { get; init; } = string.Empty;
    public string? MeasuredValue { get; init; }
    public string? Unit { get; init; }
    public string? MinStandardValue { get; init; }
    public string? MaxStandardValue { get; init; }
    public bool IsPassed { get; init; }
    public string? Remark { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed class QualityInspectionDto
{
    public Guid Id { get; init; }
    public Guid BatchId { get; init; }
    public string? BatchCode { get; init; }
    public Guid InspectorId { get; init; }
    public string? InspectorName { get; init; }
    public int InspectionType { get; init; }
    public int Status { get; init; }
    public string? OverallResult { get; init; }
    public DateTime InspectionDate { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public IReadOnlyList<InspectionLabTestDto> LabTests { get; init; }
        = Array.Empty<InspectionLabTestDto>();
}
