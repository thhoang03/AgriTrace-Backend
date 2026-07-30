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

        builder.Property(x => x.InspectionType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.OverallResult)
            .HasMaxLength(10);

        builder.Property(x => x.InspectionDate)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(x => x.UpdatedAt);

        // Batch 1 - N Inspection
        builder.HasOne(x => x.Batch)
            .WithMany(x => x.QualityInspections)
            .HasForeignKey(x => x.BatchId)
            .OnDelete(DeleteBehavior.Restrict);

        // User Inspector
        builder.HasOne(x => x.Inspector)
            .WithMany()
            .HasForeignKey(x => x.InspectorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Lab Tests 1 - N
        builder.HasMany(x => x.LabTests)
            .WithOne(x => x.Inspection)
            .HasForeignKey(x => x.InspectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}