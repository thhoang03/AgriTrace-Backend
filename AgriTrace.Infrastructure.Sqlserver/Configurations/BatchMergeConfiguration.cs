using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


public class BatchMergeConfiguration
    : IEntityTypeConfiguration<BatchMergeDataModel>
{

    public void Configure(
        EntityTypeBuilder<BatchMergeDataModel> builder)
    {


        builder.ToTable("BatchMerges");


        builder.HasKey(x => x.Id);



        builder.HasOne(x => x.NewBatch)

            .WithMany()

            .HasForeignKey(x => x.NewBatchId)

            .OnDelete(DeleteBehavior.Restrict);



        builder.HasMany(x => x.Sources)

            .WithOne(x => x.BatchMerge)

            .HasForeignKey(x => x.BatchMergeId);



    }

}