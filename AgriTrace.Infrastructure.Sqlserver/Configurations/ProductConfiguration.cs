using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AgriTrace.Infrastructure.Sqlserver.Configurations;


public class ProductConfiguration
    : IEntityTypeConfiguration<ProductDataModel>
{

    public void Configure(
        EntityTypeBuilder<ProductDataModel> builder)
    {


        builder.ToTable("Products");



        builder.HasKey(x => x.Id);



        builder.Property(x => x.Name)

            .HasMaxLength(200)

            .IsRequired();




        builder.Property(x => x.CreatedAt)

            .HasDefaultValueSql("GETDATE()");




        // Organization 1 - N Product

        builder.HasOne(x => x.Organization)

            .WithMany(x => x.Products)

            .HasForeignKey(x => x.OrganizationId)

            .OnDelete(DeleteBehavior.Restrict);




        // Category 1 - N Product

        builder.HasOne(x => x.Category)

            .WithMany(x => x.Products)

            .HasForeignKey(x => x.CategoryId)

            .OnDelete(DeleteBehavior.Restrict);




        // Unit 1 - N Product

        builder.HasOne(x => x.Unit)

            .WithMany(x => x.Products)

            .HasForeignKey(x => x.UnitId)

            .OnDelete(DeleteBehavior.Restrict);



    }

}