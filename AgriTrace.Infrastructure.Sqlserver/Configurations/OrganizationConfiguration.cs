using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrganizationConfiguration
    : IEntityTypeConfiguration<OrganizationDataModel>
{

    public void Configure(
        EntityTypeBuilder<OrganizationDataModel> builder)
    {

        builder.ToTable("Organizations");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();



        builder.Property(x => x.Address)
            .HasMaxLength(500);



        builder.Property(x => x.Status)
            .HasConversion<int>();


        builder.HasOne(x => x.OrganizationType)
            .WithMany(x => x.Organizations)
            .HasForeignKey(x => x.OrganizationTypeId)
            .OnDelete(DeleteBehavior.Restrict);

    }

}