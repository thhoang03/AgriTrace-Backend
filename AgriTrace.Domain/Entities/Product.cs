using System;

namespace AgriTrace.Domain.Entities
{
    public class Product
    {
        public Guid ProductId { get; private set; }
        public int CategoryId { get; private set; }
        public string Name { get; private set; } = null!;
        public string? ScientificName { get; private set; }
        public string? Description { get; private set; }
        public string? ImageUrl { get; private set; }
        public int? ShelfLifeDays { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Product() { }

        public Product(Guid productId, int categoryId, string name, string? scientificName, string? description, string? imageUrl, int? shelfLifeDays, bool isActive = true, DateTime? createdAt = null, DateTime? updatedAt = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty.", nameof(name));

            ProductId = productId == Guid.Empty ? Guid.NewGuid() : productId;
            CategoryId = categoryId;
            Name = name;
            ScientificName = scientificName;
            Description = description;
            ImageUrl = imageUrl;
            ShelfLifeDays = shelfLifeDays;
            IsActive = isActive;
            CreatedAt = createdAt ?? DateTime.UtcNow;
            UpdatedAt = updatedAt;
        }

        public void UpdateDetails(int categoryId, string name, string? scientificName, string? description, string? imageUrl, int? shelfLifeDays)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty.", nameof(name));

            CategoryId = categoryId;
            Name = name;
            ScientificName = scientificName;
            Description = description;
            ImageUrl = imageUrl;
            ShelfLifeDays = shelfLifeDays;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
