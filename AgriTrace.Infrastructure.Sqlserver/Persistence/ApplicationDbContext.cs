using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;


namespace AgriTrace.Infrastructure.Sqlserver.Persistence;


public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }



    // Master Data

    public DbSet<OrganizationTypeDataModel> OrganizationTypes
        => Set<OrganizationTypeDataModel>();


    public DbSet<EventTypeDataModel> EventTypes
        => Set<EventTypeDataModel>();


    public DbSet<CategoryDataModel> Categories
        => Set<CategoryDataModel>();


    public DbSet<UnitDataModel> Units
        => Set<UnitDataModel>();



    // Identity

    public DbSet<UserDataModel> Users
        => Set<UserDataModel>();



    // Business Core

    public DbSet<OrganizationDataModel> Organizations
        => Set<OrganizationDataModel>();


    public DbSet<ProductDataModel> Products
        => Set<ProductDataModel>();


    public DbSet<BatchDataModel> Batches
        => Set<BatchDataModel>();



    // Traceability

    public DbSet<SupplyChainEventDataModel> SupplyChainEvents
        => Set<SupplyChainEventDataModel>();


    public DbSet<BatchSplitDataModel> BatchSplits
        => Set<BatchSplitDataModel>();


    public DbSet<BatchSplitDetailDataModel> BatchSplitDetails
        => Set<BatchSplitDetailDataModel>();


    public DbSet<BatchMergeDataModel> BatchMerges
        => Set<BatchMergeDataModel>();


    public DbSet<BatchMergeSourceDataModel> BatchMergeSources
        => Set<BatchMergeSourceDataModel>();



    // Quality

    public DbSet<QualityInspectionDataModel> QualityInspections
        => Set<QualityInspectionDataModel>();


    public DbSet<CertificateDataModel> Certificates
        => Set<CertificateDataModel>();



    // Safety

    public DbSet<RecallDataModel> Recalls
        => Set<RecallDataModel>();


    public DbSet<NotificationDataModel> Notifications
        => Set<NotificationDataModel>();



    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);



        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

    }

}