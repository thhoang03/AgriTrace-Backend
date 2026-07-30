using System.Text.Json.Serialization;

namespace AgriTrace.API.Models;

public class LabTestResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("testName")]
    public string TestName { get; set; } = string.Empty;

    [JsonPropertyName("measuredValue")]
    public string? MeasuredValue { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("minStandardValue")]
    public string? MinStandardValue { get; set; }

    [JsonPropertyName("maxStandardValue")]
    public string? MaxStandardValue { get; set; }

    [JsonPropertyName("isPassed")]
    public bool IsPassed { get; set; }

    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

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

    [JsonPropertyName("inspectionType")]
    public int InspectionType { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("overallResult")]
    public string? OverallResult { get; set; }

    [JsonPropertyName("inspectionDate")]
    public DateTime InspectionDate { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("labTests")]
    public List<LabTestResponse> LabTests { get; set; } = new();
}
