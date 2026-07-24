using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models;

/// <summary>
/// Request body for creating/updating a product.
/// Supports both string Unit and UUID UnitId for frontend flexibility.
/// </summary>
public sealed class ProductRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Unit code/name (e.g. "kg").
    /// </summary>
    [StringLength(50)]
    public string? Unit { get; set; }

    /// <summary>
    /// Direct Unit ID (Guid).
    /// </summary>
    public Guid? UnitId { get; set; }

    public Guid? OrganizationId { get; set; }
}
