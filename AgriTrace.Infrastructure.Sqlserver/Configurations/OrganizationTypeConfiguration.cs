using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AgriTrace.Infrastructure.Sqlserver.Configurations;


public class OrganizationTypeConfiguration
    : IEntityTypeConfiguration<OrganizationTypeDataModel>
{

    public void Configure(
        EntityTypeBuilder<OrganizationTypeDataModel> builder)
    {


        builder.ToTable("OrganizationTypes");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();



        builder.HasIndex(x => x.Code)
            .IsUnique();



        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();



        builder.Property(x => x.Description)
            .HasMaxLength(500);

    }
}