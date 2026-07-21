namespace AgriTrace.Application.Contracts;

/// <summary>
/// Public traceability projection for a batch (no authentication required).
/// Mirrors swagger <c>PublicTraceData</c>.
/// </summary>
public class PublicTraceDto
{
    public Guid BatchId { get; set; }
    public string BatchCode { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public decimal Quantity { get; set; }
    public string? UnitCode { get; set; }
    public string? CurrentOrganizationName { get; set; }
    public int Status { get; set; }
    public List<TimelineEventDto> Timeline { get; set; } = new();
    public List<PublicInspectionDto> Inspections { get; set; } = new();
    public List<PublicCertificateDto> Certificates { get; set; } = new();
    public string? RecallStatus { get; set; }
}

public class TimelineEventDto
{
    public string? EventTypeCode { get; set; }
    public string? OrganizationName { get; set; }
    public DateTime EventTime { get; set; }
    public string? Location { get; set; }
}

public class PublicInspectionDto
{
    public string? Result { get; set; }
    public string? InspectorName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PublicCertificateDto
{
    public string? CertificateType { get; set; }
    public string? FileUrl { get; set; }
    public DateTime? IssuedDate { get; set; }
}

/// <summary>
/// Batch lineage projection. Mirrors swagger <c>LineageData</c>.
/// </summary>
public class LineageDto
{
    public Guid RootBatchId { get; set; }
    public List<LineageNodeDto> Lineage { get; set; } = new();
}

public class LineageNodeDto
{
    public Guid BatchId { get; set; }
    public string BatchCode { get; set; } = string.Empty;
    public string? EventTypeCode { get; set; }
    public decimal Quantity { get; set; }
    public Guid? ParentBatchId { get; set; }
}
