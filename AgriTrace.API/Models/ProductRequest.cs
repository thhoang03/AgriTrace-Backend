using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models;

/// <summary>
/// Request body for creating/updating a product. Matches swagger <c>ProductRequest</c>.
/// </summary>
/// <remarks>
/// ID Type Note: swagger declares <c>categoryId</c> and <c>organizationId</c> as integer, but the
/// domain uses <c>Guid</c> for all entity keys. Per the documented Phase 1 decision, the API keeps
/// <c>Guid</c> for these fields (Guid deviation to be annotated in Phase 11).
/// </remarks>
public sealed class ProductRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Unit code/name (e.g. "kg"). Swagger models unit as a plain string, not a UUID reference.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Unit { get; set; } = string.Empty;

    [Required]
    public Guid OrganizationId { get; set; }
}
