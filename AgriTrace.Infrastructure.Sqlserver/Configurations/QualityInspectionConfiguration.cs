using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AgriTrace.Infrastructure.Sqlserver.Configurations;


public class QualityInspectionConfiguration
    : IEntityTypeConfiguration<QualityInspectionDataModel>
{

    public void Configure(
        EntityTypeBuilder<QualityInspectionDataModel> builder)
    {


        builder.ToTable("QualityInspections");



        builder.HasKey(x => x.Id);



        builder.Property(x => x.Result)

            .HasMaxLength(500);



        builder.Property(x => x.Notes)

            .HasColumnType("nvarchar(max)");




        builder.Property(x => x.CreatedAt)

            .HasDefaultValueSql("GETDATE()");




        // Batch 1 - N Inspection

        builder.HasOne(x => x.Batch)

            .WithMany(x => x.Inspections)

            .HasForeignKey(x => x.BatchId)

            .OnDelete(DeleteBehavior.Restrict);




        // User Inspector

        builder.HasOne(x => x.Inspector)

            .WithMany()

            .HasForeignKey(x => x.InspectorId)

            .OnDelete(DeleteBehavior.Restrict);


    }

}