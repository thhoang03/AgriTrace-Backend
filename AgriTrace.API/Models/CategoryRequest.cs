using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models;

public class CategoryRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }
}
