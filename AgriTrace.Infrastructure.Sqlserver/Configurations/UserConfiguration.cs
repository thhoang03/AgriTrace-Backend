using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AgriTrace.Infrastructure.Sqlserver.Configurations;


public class UserConfiguration
    : IEntityTypeConfiguration<UserDataModel>
{

    public void Configure(
        EntityTypeBuilder<UserDataModel> builder)
    {

        builder.ToTable("Users");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.FullName)
            .HasMaxLength(200);



        builder.Property(x => x.Email)
            .HasMaxLength(200)
            .IsRequired();



        builder.HasIndex(x => x.Email)
            .IsUnique();



        builder.Property(x => x.PasswordHash)
            .HasMaxLength(500);



        builder.Property(x => x.Role)
            .HasMaxLength(50)
            .IsRequired()
            .HasConversion<string>();



        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);



        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETDATE()");




        // Organization relationship

        builder.HasOne(x => x.Organization)

            .WithMany(x => x.Users)

            .HasForeignKey(x => x.OrganizationId)

            .OnDelete(DeleteBehavior.Restrict);



    }

}