using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models;

/// <summary>
/// Paged response for categories. Matches swagger <c>CategoryPagedResponse</c> (extends <see cref="PagedMeta"/>).
/// </summary>
public class CategoryPagedResponse : PagedMeta
{
    [JsonPropertyName("items")]
    public List<CategoryResponse> Items { get; set; } = new();

    public CategoryPagedResponse()
    {
    }

    public CategoryPagedResponse(IEnumerable<CategoryResponse> items, int totalCount, int page, int pageSize)
    {
        Items = items.ToList();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = pageSize == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
