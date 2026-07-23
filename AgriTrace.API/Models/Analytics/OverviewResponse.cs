using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Analytics;

/// <summary>
/// Dashboard overview counts. Matches swagger <c>OverviewData</c>.
/// </summary>
public class OverviewData
{
    [JsonPropertyName("totalBatches")]
    public int TotalBatches { get; set; }

    [JsonPropertyName("totalOrganizations")]
    public int TotalOrganizations { get; set; }

    [JsonPropertyName("totalEvents")]
    public int TotalEvents { get; set; }

    [JsonPropertyName("totalRecalls")]
    public int TotalRecalls { get; set; }

    [JsonPropertyName("activeBatches")]
    public int ActiveBatches { get; set; }

    [JsonPropertyName("recalledBatches")]
    public int RecalledBatches { get; set; }
}
