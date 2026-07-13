using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AgriTrace.Infrastructure.Sqlserver.Configurations;


public class CertificateConfiguration
    : IEntityTypeConfiguration<CertificateDataModel>
{

    public void Configure(
        EntityTypeBuilder<CertificateDataModel> builder)
    {


        builder.ToTable("Certificates");



        builder.HasKey(x => x.Id);



        builder.Property(x => x.CertificateType)

            .HasMaxLength(100);




        builder.Property(x => x.FileUrl)

            .HasMaxLength(500);




        // Batch

        builder.HasOne(x => x.Batch)

            .WithMany(x => x.Certificates)

            .HasForeignKey(x => x.BatchId)

            .OnDelete(DeleteBehavior.Restrict);




        // Inspection optional

        builder.HasOne(x => x.Inspection)

            .WithMany(x => x.Certificates)

            .HasForeignKey(x => x.InspectionId)

            .OnDelete(DeleteBehavior.Restrict);



    }

}