using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Analytics;

/// <summary>
/// Traceback result. Matches swagger <c>TracebackData</c>.
/// </summary>
public class TracebackData
{
    [JsonPropertyName("batchId")]
    public Guid BatchId { get; set; }

    [JsonPropertyName("batchCode")]
    public string BatchCode { get; set; } = string.Empty;

    [JsonPropertyName("affectedBatches")]
    public List<AffectedBatch> AffectedBatches { get; set; } = new();

    [JsonPropertyName("relatedOrganizations")]
    public List<RelatedOrganization> RelatedOrganizations { get; set; } = new();
}

public class AffectedBatch
{
    [JsonPropertyName("batchId")]
    public Guid BatchId { get; set; }

    [JsonPropertyName("batchCode")]
    public string BatchCode { get; set; } = string.Empty;

    [JsonPropertyName("relationship")]
    public string Relationship { get; set; } = string.Empty;
}

public class RelatedOrganization
{
    [JsonPropertyName("organizationId")]
    public Guid OrganizationId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}
