using System;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models;

/// <summary>
/// Response DTO for a category. Matches swagger <c>CategoryDetail</c> / <c>CategoryListItem</c>.
/// </summary>
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

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}
