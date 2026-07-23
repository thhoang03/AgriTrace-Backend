using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models;

/// <summary>
/// Created data for a batch. Matches swagger <c>BatchCreatedData</c>.
/// </summary>
/// <remarks>
/// ID Type Note: swagger declares <c>batchId</c> as uuid string; the API keeps <c>Guid</c> (Phase 1 decision).
/// </remarks>
public class BatchCreatedData
{
    [JsonPropertyName("batchId")]
    public Guid BatchId { get; set; }

    [JsonPropertyName("batchCode")]
    public string BatchCode { get; set; } = null!;

    [JsonPropertyName("qrCodeUrl")]
    public string? QrCodeUrl { get; set; }
}

/// <summary>
/// List item batch response. Matches swagger <c>BatchListItem</c>.
/// </summary>
public class BatchListItemResponse
{
    [JsonPropertyName("batchId")]
    public Guid BatchId { get; set; }

    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }

    [JsonPropertyName("productName")]
    public string? ProductName { get; set; }

    [JsonPropertyName("batchCode")]
    public string BatchCode { get; set; } = null!;

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("unitId")]
    public Guid UnitId { get; set; }

    [JsonPropertyName("unitCode")]
    public string? UnitCode { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("statusName")]
    public string StatusName { get; set; } = null!;

    [JsonPropertyName("currentOrganizationId")]
    public Guid CurrentOrganizationId { get; set; }

    [JsonPropertyName("qrCodeUrl")]
    public string? QrCodeUrl { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Detailed batch response. Matches swagger <c>BatchDetail</c>.
/// </summary>
public class BatchDetailResponse
{
    [JsonPropertyName("batchId")]
    public Guid BatchId { get; set; }

    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }

    [JsonPropertyName("productName")]
    public string? ProductName { get; set; }

    [JsonPropertyName("categoryId")]
    public Guid? CategoryId { get; set; }

    [JsonPropertyName("categoryName")]
    public string? CategoryName { get; set; }

    [JsonPropertyName("batchCode")]
    public string BatchCode { get; set; } = null!;

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("unitId")]
    public Guid UnitId { get; set; }

    [JsonPropertyName("unitCode")]
    public string? UnitCode { get; set; }

    [JsonPropertyName("productionDate")]
    public DateOnly ProductionDate { get; set; }

    [JsonPropertyName("expiryDate")]
    public DateOnly? ExpiryDate { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("currentOrganizationId")]
    public Guid CurrentOrganizationId { get; set; }

    [JsonPropertyName("organizationName")]
    public string? OrganizationName { get; set; }

    [JsonPropertyName("qrCodeUrl")]
    public string? QrCodeUrl { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Paged response for batches. Matches swagger <c>BatchPagedResponse</c>.
/// </summary>
public class BatchPagedResponse : PagedMeta
{
    [JsonPropertyName("items")]
    public List<BatchListItemResponse> Items { get; set; } = new();

    public BatchPagedResponse()
    {
    }

    public BatchPagedResponse(IEnumerable<BatchListItemResponse> items, int totalCount, int page, int pageSize)
    {
        Items = items.ToList();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = pageSize == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
