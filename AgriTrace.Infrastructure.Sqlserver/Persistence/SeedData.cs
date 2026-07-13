using AgriTrace.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgriTrace.Infrastructure.Sqlserver.Persistence;


public static class SeedData
{

    public static void Seed(ModelBuilder builder)
    {
        SeedOrganizationTypes(builder);
        SeedEventTypes(builder);
        SeedCategories(builder);
    }


    private static void SeedOrganizationTypes(ModelBuilder builder)
    {
        builder.Entity<OrganizationTypeDataModel>().HasData(
            new OrganizationTypeDataModel
            {
                Id = new Guid("10000000-0000-0000-0000-000000000001"),
                Code = "FARM",
                Name = "Farm",
                Description = "Agricultural Farm"
            },
            new OrganizationTypeDataModel
            {
                Id = new Guid("10000000-0000-0000-0000-000000000002"),
                Code = "PROCESSOR",
                Name = "Processor"
            },
            new OrganizationTypeDataModel
            {
                Id = new Guid("10000000-0000-0000-0000-000000000003"),
                Code = "DISTRIBUTOR",
                Name = "Distributor"
            },
            new OrganizationTypeDataModel
            {
                Id = new Guid("10000000-0000-0000-0000-000000000004"),
                Code = "RETAILER",
                Name = "Retailer"
            }
        );
    }



    private static void SeedEventTypes(ModelBuilder builder)
    {
        builder.Entity<EventTypeDataModel>().HasData(
            new EventTypeDataModel { Id = new Guid("20000000-0000-0000-0000-000000000001"), Code = "HARVEST", Name = "Harvest" },
            new EventTypeDataModel { Id = new Guid("20000000-0000-0000-0000-000000000002"), Code = "RECEIVE", Name = "Receive" },
            new EventTypeDataModel { Id = new Guid("20000000-0000-0000-0000-000000000003"), Code = "PROCESSING", Name = "Processing" },
            new EventTypeDataModel { Id = new Guid("20000000-0000-0000-0000-000000000004"), Code = "PACKAGING", Name = "Packaging" },
            new EventTypeDataModel { Id = new Guid("20000000-0000-0000-0000-000000000005"), Code = "TRANSPORT", Name = "Transport" },
            new EventTypeDataModel { Id = new Guid("20000000-0000-0000-0000-000000000006"), Code = "DISTRIBUTION", Name = "Distribution" },
            new EventTypeDataModel { Id = new Guid("20000000-0000-0000-0000-000000000007"), Code = "RETAIL", Name = "Retail" },
            new EventTypeDataModel { Id = new Guid("20000000-0000-0000-0000-000000000008"), Code = "INSPECTION", Name = "Inspection" },
            new EventTypeDataModel { Id = new Guid("20000000-0000-0000-0000-000000000009"), Code = "RECALL", Name = "Recall" }
        );
    }
    private static void SeedCategories(ModelBuilder builder)
    {
        builder.Entity<CategoryDataModel>().HasData(
            new CategoryDataModel { Id = new Guid("30000000-0000-0000-0000-000000000001"), Name = "Rau củ", Description = "Nhóm rau củ quả tươi", IsActive = true },
            new CategoryDataModel { Id = new Guid("30000000-0000-0000-0000-000000000002"), Name = "Coffee", Description = "Các loại cà phê", IsActive = true },
            new CategoryDataModel { Id = new Guid("30000000-0000-0000-0000-000000000003"), Name = "Rice", Description = "Gạo các loại", IsActive = true },
            new CategoryDataModel { Id = new Guid("30000000-0000-0000-0000-000000000004"), Name = "Fruits", Description = "Trái cây tươi", IsActive = true },
            new CategoryDataModel { Id = new Guid("30000000-0000-0000-0000-000000000005"), Name = "Herbs", Description = "Thảo mộc và gia vị", IsActive = true }
        );
    }

}