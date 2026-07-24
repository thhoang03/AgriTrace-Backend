using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgriTrace.Infrastructure.Sqlserver.Configurations;

public class CategoryConfiguration
    : IEntityTypeConfiguration<CategoryDataModel>
{
    public void Configure(
        EntityTypeBuilder<CategoryDataModel> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}
