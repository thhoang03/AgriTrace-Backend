using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Analytics;

/// <summary>
/// Batch distribution by status. Matches swagger <c>BatchDistributionData</c>.
/// </summary>
public class BatchDistributionData
{
    [JsonPropertyName("items")]
    public List<BatchStatusDistributionItem> Items { get; set; } = new();

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }
}

public class BatchStatusDistributionItem
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("statusName")]
    public string StatusName { get; set; } = string.Empty;

    [JsonPropertyName("count")]
    public int Count { get; set; }
}
