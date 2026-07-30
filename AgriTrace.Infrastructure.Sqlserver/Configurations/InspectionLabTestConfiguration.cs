using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgriTrace.Infrastructure.Sqlserver.Configurations;

public class InspectionLabTestConfiguration
    : IEntityTypeConfiguration<InspectionLabTestDataModel>
{
    public void Configure(EntityTypeBuilder<InspectionLabTestDataModel> builder)
    {
        builder.ToTable("InspectionLabTests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TestName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.MeasuredValue)
            .HasMaxLength(200);

        builder.Property(x => x.Unit)
            .HasMaxLength(50);

        builder.Property(x => x.MinStandardValue)
            .HasMaxLength(200);

        builder.Property(x => x.MaxStandardValue)
            .HasMaxLength(200);

        builder.Property(x => x.Remark)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.HasOne(x => x.Inspection)
            .WithMany(x => x.LabTests)
            .HasForeignKey(x => x.InspectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
