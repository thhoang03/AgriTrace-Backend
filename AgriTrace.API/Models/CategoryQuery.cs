using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models;

public class CategoryQuery : PaginationRequest
{
    [StringLength(100)]
    public string? Search { get; set; }
}
