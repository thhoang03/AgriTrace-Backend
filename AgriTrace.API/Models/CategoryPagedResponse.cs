using System;
using System.Collections.Generic;
using System.Linq;

namespace AgriTrace.API.Models;

public class CategoryPagedResponse
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public List<CategoryResponse> Items { get; set; } = new();

    public CategoryPagedResponse()
    {
    }

    public CategoryPagedResponse(IEnumerable<CategoryResponse> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items.ToList();
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
