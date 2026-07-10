using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AgriTrace.Infrastructure.Sqlserver.Configurations;


public class UnitConfiguration
    : IEntityTypeConfiguration<UnitDataModel>
{

    public void Configure(
        EntityTypeBuilder<UnitDataModel> builder)
    {


        builder.ToTable("Units");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.Code)
            .HasMaxLength(20);



        builder.HasIndex(x => x.Code)
            .IsUnique();



        builder.Property(x => x.Name)
            .HasMaxLength(100);



    }

}