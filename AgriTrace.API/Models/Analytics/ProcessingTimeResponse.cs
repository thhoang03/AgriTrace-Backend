using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Analytics;

/// <summary>
/// Processing-time analytics. Matches swagger <c>ProcessingTimeData</c>.
/// </summary>
public class ProcessingTimeData
{
    [JsonPropertyName("averageProcessingHours")]
    public double AverageProcessingHours { get; set; }

    [JsonPropertyName("byEventType")]
    public List<ProcessingTimeByEventType> ByEventType { get; set; } = new();
}

public class ProcessingTimeByEventType
{
    [JsonPropertyName("eventTypeCode")]
    public string? EventTypeCode { get; set; }

    [JsonPropertyName("averageHours")]
    public double AverageHours { get; set; }
}
