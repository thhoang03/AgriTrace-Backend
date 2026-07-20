using System;
using System.Collections.Generic;
using System.Linq;

namespace AgriTrace.API.Models;

/// <summary>
/// Paged response for certificates. Matches swagger <c>CertificatePagedResponse</c>.
/// </summary>
public class CertificatePagedResponse : PagedMeta
{
    public List<CertificateResponse> Items { get; set; } = new();

    public CertificatePagedResponse()
    {
    }

    public CertificatePagedResponse(IEnumerable<CertificateResponse> items, int totalCount, int page, int pageSize)
    {
        Items = items.ToList();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = pageSize == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
