using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models;

/// <summary>
/// Detailed organization response. Matches swagger <c>OrganizationDetail</c>.
/// </summary>
/// <remarks>
/// ID Type Note: swagger declares <c>organizationId</c> as integer, but the domain uses <c>Guid</c>.
/// Per the documented Phase 1 decision the API keeps <c>Guid</c> (annotation deferred to Phase 11).
/// </remarks>
public class OrganizationDetailResponse
{
    [JsonPropertyName("organizationId")]
    public Guid OrganizationId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
}

/// <summary>
/// List item organization response. Matches swagger <c>OrganizationListItem</c>.
/// </summary>
public class OrganizationListItem
{
    [JsonPropertyName("organizationId")]
    public Guid OrganizationId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
}

/// <summary>
/// Paged response for organizations. Matches swagger <c>OrganizationPagedResponse</c>.
/// </summary>
public class OrganizationPagedResponse : PagedMeta
{
    [JsonPropertyName("items")]
    public List<OrganizationListItem> Items { get; set; } = new();

    public OrganizationPagedResponse()
    {
    }

    public OrganizationPagedResponse(IEnumerable<OrganizationListItem> items, int totalCount, int page, int pageSize)
    {
        Items = items.ToList();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = pageSize == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}

/// <summary>
/// Created data for an organization. Matches swagger <c>OrganizationCreatedData</c>.
/// </summary>
public class OrganizationCreatedData
{
    [JsonPropertyName("organizationId")]
    public Guid OrganizationId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
