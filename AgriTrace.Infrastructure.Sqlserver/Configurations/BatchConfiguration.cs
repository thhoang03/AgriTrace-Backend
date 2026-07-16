using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AgriTrace.Infrastructure.Sqlserver.Configurations;


public sealed class BatchConfiguration
    : IEntityTypeConfiguration<BatchDataModel>
{

    public void Configure(
        EntityTypeBuilder<BatchDataModel> builder)
    {


        // =========================
        // Table
        // =========================

        builder.ToTable("Batches");



        builder.HasKey(x => x.Id);






        // =========================
        // Properties
        // =========================


        builder.Property(x => x.BatchCode)

            .HasMaxLength(100)

            .IsRequired();



        builder.HasIndex(x => x.BatchCode)

            .IsUnique();





        builder.Property(x => x.QRCode)

            .HasMaxLength(500)

            .IsRequired(false);



        builder.HasIndex(x => x.QRCode);






        builder.Property(x => x.Quantity)

            .HasPrecision(18, 2)

            .IsRequired();




        builder.Property(x => x.RemainingQuantity)

            .HasPrecision(18, 2)

            .IsRequired();





        builder.Property(x => x.SourceQuantity)

            .HasPrecision(18, 2)

            .IsRequired();







        builder.Property(x => x.Status)

            .HasConversion<int>()

            .IsRequired();







        builder.Property(x => x.CreatedAt)

            .HasDefaultValueSql("GETDATE()");




        builder.Property(x => x.UpdatedAt)

            .IsRequired(false);









        // =========================
        // Product Relationship
        // Product 1 - N Batch
        // =========================


        builder.HasOne(x => x.Product)

            .WithMany(x => x.Batches)

            .HasForeignKey(x => x.ProductId)

            .OnDelete(DeleteBehavior.Restrict);









        // =========================
        // Organization Relationship
        // Organization 1 - N Batch
        // =========================


        builder.HasOne(x => x.CurrentOrganization)

            .WithMany()

            .HasForeignKey(x => x.CurrentOrganizationId)

            .OnDelete(DeleteBehavior.Restrict);









        // =========================
        // Unit Relationship
        // Unit 1 - N Batch
        // =========================


        builder.HasOne(x => x.Unit)

            .WithMany()

            .HasForeignKey(x => x.UnitId)

            .OnDelete(DeleteBehavior.Restrict);









        // =========================
        // Self Reference
        //
        // Parent Batch
        //       |
        //       |
        // Child Batches
        //
        // =========================


        builder.HasOne(x => x.ParentBatch)

            .WithMany(x => x.ChildBatches)

            .HasForeignKey(x => x.ParentBatchId)

            .OnDelete(DeleteBehavior.Restrict);









        // =========================
        // Supply Chain Events
        // =========================


        builder.HasMany(x => x.Events)

            .WithOne(x => x.Batch)

            .HasForeignKey(x => x.BatchId)

            .OnDelete(DeleteBehavior.Restrict);









        // =========================
        // Quality Inspections
        // =========================


        builder.HasMany(x => x.QualityInspections)

            .WithOne(x => x.Batch)

            .HasForeignKey(x => x.BatchId)

            .OnDelete(DeleteBehavior.Restrict);









        // =========================
        // Certificates
        // =========================


        builder.HasMany(x => x.Certificates)

            .WithOne(x => x.Batch)

            .HasForeignKey(x => x.BatchId)

            .OnDelete(DeleteBehavior.Restrict);









        // =========================
        // Recalls
        // =========================


        builder.HasMany(x => x.Recalls)

            .WithOne(x => x.Batch)

            .HasForeignKey(x => x.BatchId)

            .OnDelete(DeleteBehavior.Restrict);









        // =========================
        // Traceability Index
        // =========================


        builder.HasIndex(x => x.RootBatchId);



        builder.HasIndex(x => x.ParentBatchId);



        builder.HasIndex(x => x.SplitId);



        builder.HasIndex(x => x.ProductId);



        builder.HasIndex(x => x.CurrentOrganizationId);

    }

}