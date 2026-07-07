using System;

namespace AgriTrace.Infrastructure.Sqlserver.Models
{
    public class ProductDataModel
    {
        public Guid ProductId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? ScientificName { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int? ShelfLifeDays { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
