namespace AgriTrace.Application.Contracts;

public sealed class QualityInspectionDto
{
    public Guid Id { get; init; }

    public Guid BatchId { get; init; }

    public string? BatchCode { get; init; }

    public Guid InspectorId { get; init; }

    public string? InspectorName { get; init; }

    public int Status { get; init; }

    public string? Result { get; init; }

    public string? Notes { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }
}
