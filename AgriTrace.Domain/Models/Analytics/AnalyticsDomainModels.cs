namespace AgriTrace.Domain.Models.Analytics;

public class OverviewResult
{
    public int TotalBatches { get; set; }
    public int TotalOrganizations { get; set; }
    public int TotalEvents { get; set; }
    public int TotalRecalls { get; set; }
    public int ActiveBatches { get; set; }
    public int RecalledBatches { get; set; }
    
    public List<MonthlyProductionResult> MonthlyProduction { get; set; } = new();
    public List<BatchStatusDistributionResult> BatchStatus { get; set; } = new();
    public List<InspectionResultData> InspectionResults { get; set; } = new();
    public List<RecallTrendResult> RecallTrend { get; set; } = new();
}

public class MonthlyProductionResult
{
    public string Month { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public int Batches { get; set; }
}

public class BatchStatusDistributionResult
{
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class InspectionResultData
{
    public string Month { get; set; } = string.Empty;
    public int Pass { get; set; }
    public int Fail { get; set; }
    public int Pending { get; set; }
}

public class RecallTrendResult
{
    public string Month { get; set; } = string.Empty;
    public int Recalls { get; set; }
}

public class BatchDistributionResult
{
    public List<BatchStatusDistributionResult> Items { get; set; } = new();
    public int TotalCount { get; set; }
}

public class ProcessingTimeResult
{
    public double AverageProcessingHours { get; set; }
    public List<ProcessingTimeByEventTypeResult> ByEventType { get; set; } = new();
}

public class ProcessingTimeByEventTypeResult
{
    public string? EventTypeCode { get; set; }
    public double AverageHours { get; set; }
}
