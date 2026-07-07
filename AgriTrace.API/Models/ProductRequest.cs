using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models
{
    public class ProductRequest
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        [StringLength(200)]
        public string? ScientificName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public int? ShelfLifeDays { get; set; }
    }
}
