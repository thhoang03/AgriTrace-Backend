using System;
using System.Collections.Generic;
using System.Linq;

namespace AgriTrace.API.Models;

/// <summary>
/// Paged response for inspections. Matches swagger <c>InspectionPagedResponse</c>.
/// </summary>
public class InspectionPagedResponse : PagedMeta
{
    public List<InspectionResponse> Items { get; set; } = new();

    public InspectionPagedResponse()
    {
    }

    public InspectionPagedResponse(IEnumerable<InspectionResponse> items, int totalCount, int page, int pageSize)
    {
        Items = items.ToList();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = pageSize == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
