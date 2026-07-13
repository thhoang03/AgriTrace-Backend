using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BatchMergeSourceConfiguration
    : IEntityTypeConfiguration<BatchMergeSourceDataModel>
{

    public void Configure(
        EntityTypeBuilder<BatchMergeSourceDataModel> builder)
    {


        builder.ToTable("BatchMergeSources");



        builder.HasKey(x => new
        {
            x.BatchMergeId,
            x.SourceBatchId
        });



        builder.Property(x => x.Quantity)

            .HasPrecision(18, 2);



        builder.HasOne(x => x.SourceBatch)

            .WithMany()

            .HasForeignKey(x => x.SourceBatchId)

            .OnDelete(DeleteBehavior.Restrict);


    }

}