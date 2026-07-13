using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BatchSplitDetailConfiguration
    : IEntityTypeConfiguration<BatchSplitDetailDataModel>
{

    public void Configure(
        EntityTypeBuilder<BatchSplitDetailDataModel> builder)
    {

        builder.ToTable("BatchSplitDetails");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.Quantity)

            .HasPrecision(18, 2);



        builder.HasOne(x => x.TargetBatch)

            .WithMany()

            .HasForeignKey(x => x.TargetBatchId)

            .OnDelete(DeleteBehavior.Restrict);

    }

}