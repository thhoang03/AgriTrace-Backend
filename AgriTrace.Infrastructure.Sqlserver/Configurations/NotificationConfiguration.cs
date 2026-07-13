using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AgriTrace.Infrastructure.Sqlserver.Configurations;


public class NotificationConfiguration
    : IEntityTypeConfiguration<NotificationDataModel>
{

    public void Configure(
        EntityTypeBuilder<NotificationDataModel> builder)
    {


        builder.ToTable("Notifications");



        builder.HasKey(x => x.Id);



        builder.Property(x => x.Id)
            .ValueGeneratedNever();



        builder.Property(x => x.Title)
            .HasMaxLength(200);



        builder.Property(x => x.Message)
            .IsRequired();



        builder.Property(x => x.IsRead)
            .HasDefaultValue(false);



        builder.HasOne(x => x.User)

            .WithMany(x => x.Notifications)

            .HasForeignKey(x => x.UserId)

            .OnDelete(DeleteBehavior.Cascade);


    }

}