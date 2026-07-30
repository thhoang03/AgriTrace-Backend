using System.Collections.Generic;
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

    [JsonPropertyName("monthlyProduction")]
    public List<MonthlyProductionData> MonthlyProduction { get; set; } = new();

    [JsonPropertyName("batchStatus")]
    public List<BatchStatusDistributionItem> BatchStatus { get; set; } = new();

    [JsonPropertyName("inspectionResults")]
    public List<InspectionResultData> InspectionResults { get; set; } = new();

    [JsonPropertyName("recallTrend")]
    public List<RecallTrendData> RecallTrend { get; set; } = new();
}

public class MonthlyProductionData
{
    [JsonPropertyName("month")]
    public string Month { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("batches")]
    public int Batches { get; set; }
}

public class InspectionResultData
{
    [JsonPropertyName("month")]
    public string Month { get; set; } = string.Empty;

    [JsonPropertyName("pass")]
    public int Pass { get; set; }

    [JsonPropertyName("fail")]
    public int Fail { get; set; }

    [JsonPropertyName("pending")]
    public int Pending { get; set; }
}

public class RecallTrendData
{
    [JsonPropertyName("month")]
    public string Month { get; set; } = string.Empty;

    [JsonPropertyName("recalls")]
    public int Recalls { get; set; }
}
