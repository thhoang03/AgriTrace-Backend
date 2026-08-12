using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models
{
    /// <summary>
    /// Request body for creating/updating an organization. Matches swagger <c>OrganizationRequest</c>.
    /// </summary>
    public class OrganizationRequest
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Organization type ID (Guid from GET /api/v1/organization-types).
        /// </summary>
        [Required]
        public Guid OrganizationTypeId { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }
    }
}
