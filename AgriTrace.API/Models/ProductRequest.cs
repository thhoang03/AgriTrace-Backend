using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models;

public sealed class ProductRequest
{
    [Required]
    public Guid OrganizationId { get; set; }

    public Guid? CategoryId { get; set; }

    public Guid? UnitId { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
}