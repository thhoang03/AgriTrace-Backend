using System;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models;

/// <summary>
/// Response DTO for a category. Matches swagger <c>CategoryDetail</c> / <c>CategoryListItem</c>.
/// </summary>
/// <remarks>
/// ID Type Note: swagger declares <c>categoryId</c> as integer, but the domain uses <c>Guid</c>.
/// Per the documented Phase 1 decision the API keeps <c>Guid</c> (annotation deferred to Phase 11).
/// </remarks>
public class CategoryResponse
{
    [JsonPropertyName("categoryId")]
    public Guid CategoryId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
}
