using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models
{
    public class OrganizationRequest
    {
        [Required]
        [StringLength(150, MinimumLength = 1)]
        public string OrganizationName { get; set; }

        [Required]
        [StringLength(150,MinimumLength =1)]
        public string Address { get; set; }

        [Required]
        public Guid OrganizationTypeId { get; set; }

    }
}
