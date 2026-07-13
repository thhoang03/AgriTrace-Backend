using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AgriTrace.Infrastructure.Sqlserver.Configurations;


public class SupplyChainEventConfiguration
    : IEntityTypeConfiguration<SupplyChainEventDataModel>
{

    public void Configure(
        EntityTypeBuilder<SupplyChainEventDataModel> builder)
    {


        builder.ToTable("SupplyChainEvents");



        builder.HasKey(x => x.Id);




        builder.Property(x => x.EventData)

            .HasColumnType("nvarchar(max)");




        builder.Property(x => x.Location)

            .HasMaxLength(200);




        builder.Property(x => x.PreviousHash)

            .HasMaxLength(500);



        builder.Property(x => x.CurrentHash)

            .HasMaxLength(500);




        builder.Property(x => x.EventTime)

            .HasDefaultValueSql("GETDATE()");




        // Batch relationship

        builder.HasOne(x => x.Batch)

            .WithMany(x => x.Events)

            .HasForeignKey(x => x.BatchId)

            .OnDelete(DeleteBehavior.Restrict);




        // EventType

        builder.HasOne(x => x.EventType)

            .WithMany(x => x.Events)

            .HasForeignKey(x => x.EventTypeId)

            .OnDelete(DeleteBehavior.Restrict);




        // Organization

        builder.HasOne(x => x.Organization)

            .WithMany()

            .HasForeignKey(x => x.OrganizationId)

            .OnDelete(DeleteBehavior.Restrict);




        // User

        builder.HasOne(x => x.PerformedByUser)

            .WithMany()

            .HasForeignKey(x => x.PerformedByUserId)

            .OnDelete(DeleteBehavior.Restrict);



    }

}