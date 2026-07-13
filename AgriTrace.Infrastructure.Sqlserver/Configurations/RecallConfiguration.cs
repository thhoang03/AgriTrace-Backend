using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AgriTrace.Infrastructure.Sqlserver.Configurations;


public class RecallConfiguration
    : IEntityTypeConfiguration<RecallDataModel>
{

    public void Configure(
        EntityTypeBuilder<RecallDataModel> builder)
    {


        builder.ToTable("Recalls");



        builder.HasKey(x => x.Id);



        builder.Property(x => x.Reason)

            .HasMaxLength(500);



        builder.Property(x => x.CreatedAt)

            .HasDefaultValueSql("GETDATE()");




        builder.HasOne(x => x.Batch)

            .WithMany(x => x.Recalls)

            .HasForeignKey(x => x.BatchId)

            .OnDelete(DeleteBehavior.Restrict);




        builder.HasOne(x => x.Creator)

            .WithMany()

            .HasForeignKey(x => x.CreatedBy)

            .OnDelete(DeleteBehavior.Restrict);



    }

}