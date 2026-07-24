using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models;

public class CategoryQuery : PaginationRequest
{
    [StringLength(100)]
    public string? Search { get; set; }

    [RegularExpression("^(name|createdAt)$", ErrorMessage = "sortBy must be 'name' or 'createdAt'.")]
    public string? SortBy { get; set; }

    [RegularExpression("^(asc|desc)$", ErrorMessage = "sortDir must be 'asc' or 'desc'.")]
    public string? SortDir { get; set; }
}
