namespace AgriTrace.Application.Contracts;

/// <summary>
/// Aggregate dashboard counts. Mirrors swagger <c>OverviewData</c>.
/// </summary>
public class OverviewDto
{
    public int TotalBatches { get; set; }
    public int TotalOrganizations { get; set; }
    public int TotalEvents { get; set; }
    public int TotalRecalls { get; set; }
    public int ActiveBatches { get; set; }
    public int RecalledBatches { get; set; }
}

/// <summary>
/// Batch distribution by status. Mirrors swagger <c>BatchDistributionData</c>.
/// </summary>
public class BatchDistributionDto
{
    public List<BatchStatusDistributionItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
}

public class BatchStatusDistributionItemDto
{
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>
/// Processing time analytics. Mirrors swagger <c>ProcessingTimeData</c>.
/// </summary>
public class ProcessingTimeDto
{
    public double AverageProcessingHours { get; set; }
    public List<ProcessingTimeByEventTypeDto> ByEventType { get; set; } = new();
}

public class ProcessingTimeByEventTypeDto
{
    public string? EventTypeCode { get; set; }
    public double AverageHours { get; set; }
}

/// <summary>
/// Traceback result. Mirrors swagger <c>TracebackData</c>.
/// </summary>
public class TracebackDto
{
    public Guid BatchId { get; set; }
    public string BatchCode { get; set; } = string.Empty;
    public List<AffectedBatchDto> AffectedBatches { get; set; } = new();
    public List<RelatedOrganizationDto> RelatedOrganizations { get; set; } = new();
}

public class AffectedBatchDto
{
    public Guid BatchId { get; set; }
    public string BatchCode { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
}

public class RelatedOrganizationDto
{
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Type { get; set; }
}
