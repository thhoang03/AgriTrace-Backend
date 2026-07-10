using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AgriTrace.Infrastructure.Sqlserver.Configurations;


public class BatchConfiguration
    : IEntityTypeConfiguration<BatchDataModel>
{

    public void Configure(
        EntityTypeBuilder<BatchDataModel> builder)
    {


        builder.ToTable("Batches");



        builder.HasKey(x => x.Id);



        builder.Property(x => x.BatchCode)

            .HasMaxLength(100)

            .IsRequired();



        builder.HasIndex(x => x.BatchCode)

            .IsUnique();




        builder.Property(x => x.QRCode)

            .HasMaxLength(500);




        builder.Property(x => x.Quantity)

            .HasPrecision(18, 2);



        builder.Property(x => x.RemainingQuantity)

            .HasPrecision(18, 2);




        builder.Property(x => x.CreatedAt)

            .HasDefaultValueSql("GETDATE()");




        // Product -> Batch

        builder.HasOne(x => x.Product)

            .WithMany(x => x.Batches)

            .HasForeignKey(x => x.ProductId)

            .OnDelete(DeleteBehavior.Restrict);




        // Current Organization -> Batch

        builder.HasOne(x => x.CurrentOrganization)

            .WithMany()

            .HasForeignKey(x => x.CurrentOrganizationId)

            .OnDelete(DeleteBehavior.Restrict);




        // Parent Batch Self Reference

        builder.HasOne(x => x.ParentBatch)

            .WithMany(x => x.ChildBatches)

            .HasForeignKey(x => x.ParentBatchId)

            .OnDelete(DeleteBehavior.Restrict);



    }

}