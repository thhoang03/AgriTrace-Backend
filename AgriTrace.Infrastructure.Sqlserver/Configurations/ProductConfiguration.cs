using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgriTrace.Infrastructure.Sqlserver.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<ProductDataModel>
    {
        public void Configure(EntityTypeBuilder<ProductDataModel> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.ProductId);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.ScientificName)
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.ImageUrl)
                .HasMaxLength(500);

            builder.Property(p => p.IsActive)
                .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                .IsRequired();
        }
    }
}
