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
        /// Organization type. One of: FARM, PROCESSOR, DISTRIBUTOR, RETAILER, INSPECTOR_ORG.
        /// </summary>
        [Required]
        public string Type { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }
    }
}
