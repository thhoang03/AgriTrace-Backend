using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AgriTrace.Infrastructure.Sqlserver.Configurations;


public class BatchSplitConfiguration
    : IEntityTypeConfiguration<BatchSplitDataModel>
{

    public void Configure(
        EntityTypeBuilder<BatchSplitDataModel> builder)
    {


        builder.ToTable("BatchSplits");


        builder.HasKey(x => x.Id);



        builder.HasOne(x => x.SourceBatch)

            .WithMany()

            .HasForeignKey(x => x.SourceBatchId)

            .OnDelete(DeleteBehavior.Restrict);



        builder.HasMany(x => x.Details)

            .WithOne(x => x.Split)

            .HasForeignKey(x => x.SplitId);



    }

}