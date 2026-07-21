using System;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models;

/// <summary>
/// Response DTO for a quality inspection record. Matches swagger <c>InspectionDetail</c>.
/// </summary>
public class InspectionResponse
{
    [JsonPropertyName("inspectionId")]
    public Guid InspectionId { get; set; }

    [JsonPropertyName("batchId")]
    public Guid BatchId { get; set; }

    [JsonPropertyName("batchCode")]
    public string? BatchCode { get; set; }

    [JsonPropertyName("inspectorId")]
    public Guid InspectorId { get; set; }

    [JsonPropertyName("inspectorName")]
    public string? InspectorName { get; set; }

    /// <summary>
    /// Numeric status: 1 = Pending, 2 = Passed, 3 = Failed.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>
    /// String result: "PASS", "FAIL", or null when still Pending.
    /// </summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}
