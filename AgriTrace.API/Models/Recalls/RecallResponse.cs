using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models.Recalls;

/// <summary>
/// Detailed recall response. Matches swagger <c>RecallDetail</c>.
/// </summary>
public class RecallDetail
{
    [JsonPropertyName("recallId")]
    public Guid RecallId { get; set; }

    [JsonPropertyName("id")]
    public Guid Id => RecallId;

    [JsonPropertyName("batchId")]
    public Guid BatchId { get; set; }

    [JsonPropertyName("batchCode")]
    public string? BatchCode { get; set; }

    [JsonPropertyName("productId")]
    public Guid? ProductId { get; set; }

    [JsonPropertyName("productName")]
    public string? ProductName { get; set; }

    [JsonPropertyName("organizationId")]
    public Guid? OrganizationId { get; set; }

    [JsonPropertyName("organizationName")]
    public string? OrganizationName { get; set; }

    [JsonPropertyName("createdBy")]
    public Guid CreatedBy { get; set; }

    [JsonPropertyName("createdByName")]
    public string? CreatedByName { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    [JsonPropertyName("severity")]
    public int Severity { get; set; }

    [JsonPropertyName("severityName")]
    public string SeverityName { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("statusName")]
    public string StatusName { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Paged response for recalls. Matches swagger <c>RecallPagedResponse</c>.
/// </summary>
public class RecallPagedResponse : PagedMeta
{
    [JsonPropertyName("items")]
    public List<RecallDetail> Items { get; set; } = new();

    public RecallPagedResponse()
    {
    }

    public RecallPagedResponse(IEnumerable<RecallDetail> items, int totalCount, int page, int pageSize)
    {
        Items = items.ToList();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = pageSize == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
