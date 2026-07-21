using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Public;

/// <summary>
/// Public traceability payload. Matches swagger <c>PublicTraceData</c>.
/// </summary>
public class PublicTraceData
{
    [JsonPropertyName("batchId")]
    public Guid BatchId { get; set; }

    [JsonPropertyName("batchCode")]
    public string BatchCode { get; set; } = string.Empty;

    [JsonPropertyName("productName")]
    public string? ProductName { get; set; }

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("unitCode")]
    public string? UnitCode { get; set; }

    [JsonPropertyName("currentOrganizationName")]
    public string? CurrentOrganizationName { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("timeline")]
    public List<TimelineEvent> Timeline { get; set; } = new();

    [JsonPropertyName("inspections")]
    public List<PublicInspectionSummary> Inspections { get; set; } = new();

    [JsonPropertyName("certificates")]
    public List<PublicCertificateSummary> Certificates { get; set; } = new();

    [JsonPropertyName("recallStatus")]
    public string? RecallStatus { get; set; }
}

public class TimelineEvent
{
    [JsonPropertyName("eventTypeCode")]
    public string? EventTypeCode { get; set; }

    [JsonPropertyName("organizationName")]
    public string? OrganizationName { get; set; }

    [JsonPropertyName("eventTime")]
    public DateTime EventTime { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }
}

public class PublicInspectionSummary
{
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    [JsonPropertyName("inspectorName")]
    public string? InspectorName { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

public class PublicCertificateSummary
{
    [JsonPropertyName("certificateType")]
    public string? CertificateType { get; set; }

    [JsonPropertyName("fileUrl")]
    public string? FileUrl { get; set; }

    [JsonPropertyName("issuedDate")]
    public DateTime? IssuedDate { get; set; }
}

/// <summary>
/// Batch lineage payload. Matches swagger <c>LineageData</c>.
/// </summary>
public class LineageData
{
    [JsonPropertyName("rootBatchId")]
    public Guid RootBatchId { get; set; }

    [JsonPropertyName("lineage")]
    public List<LineageNode> Lineage { get; set; } = new();
}

public class LineageNode
{
    [JsonPropertyName("batchId")]
    public Guid BatchId { get; set; }

    [JsonPropertyName("batchCode")]
    public string BatchCode { get; set; } = string.Empty;

    [JsonPropertyName("eventTypeCode")]
    public string? EventTypeCode { get; set; }

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("parentBatchId")]
    public Guid? ParentBatchId { get; set; }
}
