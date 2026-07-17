using AgriTrace.Domain.Common.Enums;
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
        SeedUnits(builder);
        SeedOrganizations(builder);
        SeedProducts(builder);
        SeedUsers(builder);
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
            },
            new OrganizationTypeDataModel
            {
                Id = new Guid("10000000-0000-0000-0000-000000000005"),
                Code = "INSPECTION",
                Name = "Inspection"
            },
            new OrganizationTypeDataModel
            {
                Id = new Guid("10000000-0000-0000-0000-000000000006"),
                Code = "SYSTEM",
                Name = "System"
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
            new CategoryDataModel { Id = new Guid("30000000-0000-0000-0000-000000000001"), Name = "Vegetables", Description = "Fresh vegetables and tubers", IsActive = true },
            new CategoryDataModel { Id = new Guid("30000000-0000-0000-0000-000000000002"), Name = "Coffee", Description = "Various types of coffee", IsActive = true },
            new CategoryDataModel { Id = new Guid("30000000-0000-0000-0000-000000000003"), Name = "Rice", Description = "Various types of rice", IsActive = true },
            new CategoryDataModel { Id = new Guid("30000000-0000-0000-0000-000000000004"), Name = "Fruits", Description = "Fresh fruits", IsActive = true },
            new CategoryDataModel { Id = new Guid("30000000-0000-0000-0000-000000000005"), Name = "Herbs", Description = "Herbs and spices", IsActive = true }
        );
    }

    private static void SeedUnits(ModelBuilder builder)
    {
        builder.Entity<UnitDataModel>().HasData(
            new UnitDataModel
            {
                Id = new Guid("40000000-0000-0000-0000-000000000001"),
                Code = "KG",
                Name = "Kilogram",
                Symbol = "kg",
                Description = "Metric unit of mass equal to 1000 grams",
                Category = UnitCategory.Weight,
                ConversionToBase = 1m,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new UnitDataModel
            {
                Id = new Guid("40000000-0000-0000-0000-000000000002"),
                Code = "GRAM",
                Name = "Gram",
                Symbol = "g",
                Description = "Metric unit of mass equal to 1/1000 kilogram",
                Category = UnitCategory.Weight,
                ConversionToBase = 0.001m,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new UnitDataModel
            {
                Id = new Guid("40000000-0000-0000-0000-000000000003"),
                Code = "LITER",
                Name = "Liter",
                Symbol = "L",
                Description = "Metric unit of volume equal to 1000 cubic centimeters",
                Category = UnitCategory.Volume,
                ConversionToBase = 1m,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new UnitDataModel
            {
                Id = new Guid("40000000-0000-0000-0000-000000000004"),
                Code = "MILLILITER",
                Name = "Milliliter",
                Symbol = "mL",
                Description = "Metric unit of volume equal to 1/1000 liter",
                Category = UnitCategory.Volume,
                ConversionToBase = 0.001m,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new UnitDataModel
            {
                Id = new Guid("40000000-0000-0000-0000-000000000005"),
                Code = "BOX",
                Name = "Box",
                Symbol = "box",
                Description = "Packaging unit containing a fixed number of items",
                Category = UnitCategory.Count,
                ConversionToBase = 1m,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new UnitDataModel
            {
                Id = new Guid("40000000-0000-0000-0000-000000000006"),
                Code = "BALE",
                Name = "Bale",
                Symbol = "bale",
                Description = "Compressed bundle of agricultural produce",
                Category = UnitCategory.Count,
                ConversionToBase = 1m,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new UnitDataModel
            {
                Id = new Guid("40000000-0000-0000-0000-000000000007"),
                Code = "PIECE",
                Name = "Piece",
                Symbol = "pc",
                Description = "Individual countable item",
                Category = UnitCategory.Count,
                ConversionToBase = 1m,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new UnitDataModel
            {
                Id = new Guid("40000000-0000-0000-0000-000000000008"),
                Code = "TON",
                Name = "Metric Ton",
                Symbol = "t",
                Description = "Metric unit of mass equal to 1000 kilograms",
                Category = UnitCategory.Weight,
                ConversionToBase = 1000m,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new UnitDataModel
            {
                Id = new Guid("40000000-0000-0000-0000-000000000009"),
                Code = "SACK",
                Name = "Sack",
                Symbol = "sack",
                Description = "Standard sack for bulk produce, typically 50 kg",
                Category = UnitCategory.Weight,
                ConversionToBase = 50m,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }


    private static void SeedOrganizations(ModelBuilder builder)
    {
        builder.Entity<OrganizationDataModel>().HasData(
            new OrganizationDataModel
            {
                Id = new Guid("50000000-0000-0000-0000-000000000001"),
                OrganizationTypeId = new Guid("10000000-0000-0000-0000-000000000001"),
                Name = "Green Farm Co.",
                Address = "Tan Lac, Hoa Binh Province",
                Status = OrganizationStatus.Active,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new OrganizationDataModel
            {
                Id = new Guid("50000000-0000-0000-0000-000000000002"),
                OrganizationTypeId = new Guid("10000000-0000-0000-0000-000000000002"),
                Name = "Golden Bean Processor",
                Address = "Buon Ma Thuot, Dak Lak Province",
                Status = OrganizationStatus.Active,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new OrganizationDataModel
            {
                Id = new Guid("50000000-0000-0000-0000-000000000003"),
                OrganizationTypeId = new Guid("10000000-0000-0000-0000-000000000003"),
                Name = "Fresh Link Distributor",
                Address = "Binh Tan, Ho Chi Minh City",
                Status = OrganizationStatus.Active,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new OrganizationDataModel
            {
                Id = new Guid("50000000-0000-0000-0000-000000000004"),
                OrganizationTypeId = new Guid("10000000-0000-0000-0000-000000000005"),
                Name = "Agri Quality Inspection",
                Address = "Cau Giay, Hanoi City",
                Status = OrganizationStatus.Active,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }


    private static void SeedProducts(ModelBuilder builder)
    {
        builder.Entity<ProductDataModel>().HasData(
            new ProductDataModel
            {
                Id = new Guid("60000000-0000-0000-0000-000000000001"),
                OrganizationId = new Guid("50000000-0000-0000-0000-000000000001"),
                CategoryId = new Guid("30000000-0000-0000-0000-000000000001"),
                UnitId = new Guid("40000000-0000-0000-0000-000000000001"),
                Name = "Organic Tomato",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductDataModel
            {
                Id = new Guid("60000000-0000-0000-0000-000000000002"),
                OrganizationId = new Guid("50000000-0000-0000-0000-000000000001"),
                CategoryId = new Guid("30000000-0000-0000-0000-000000000004"),
                UnitId = new Guid("40000000-0000-0000-0000-000000000004"),
                Name = "Dragon Fruit",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductDataModel
            {
                Id = new Guid("60000000-0000-0000-0000-000000000003"),
                OrganizationId = new Guid("50000000-0000-0000-0000-000000000002"),
                CategoryId = new Guid("30000000-0000-0000-0000-000000000002"),
                UnitId = new Guid("40000000-0000-0000-0000-000000000005"),
                Name = "Arabica Coffee",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductDataModel
            {
                Id = new Guid("60000000-0000-0000-0000-000000000004"),
                OrganizationId = new Guid("50000000-0000-0000-0000-000000000002"),
                CategoryId = new Guid("30000000-0000-0000-0000-000000000003"),
                UnitId = new Guid("40000000-0000-0000-0000-000000000002"),
                Name = "Jasmine Rice",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }


    private static void SeedUsers(ModelBuilder builder)
    {
        builder.Entity<UserDataModel>().HasData(
            new UserDataModel
            {
                Id = new Guid("70000000-0000-0000-0000-000000000001"),
                OrganizationId = null,
                FullName = "System Administrator",
                Email = "admin@agritrace.com",
                PasswordHash = "123",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new UserDataModel
            {
                Id = new Guid("70000000-0000-0000-0000-000000000002"),
                OrganizationId = new Guid("50000000-0000-0000-0000-000000000001"),
                FullName = "Nguyen Van A",
                Email = "farmer.a@greenfarm.com",
                PasswordHash = "123",
                Role = UserRole.Farmer,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new UserDataModel
            {
                Id = new Guid("70000000-0000-0000-0000-000000000003"),
                OrganizationId = new Guid("50000000-0000-0000-0000-000000000002"),
                FullName = "Tran Thi B",
                Email = "manager.b@goldenbean.com",
                PasswordHash = "123",
                Role = UserRole.Manager,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new UserDataModel
            {
                Id = new Guid("70000000-0000-0000-0000-000000000004"),
                OrganizationId = new Guid("50000000-0000-0000-0000-000000000004"),
                FullName = "Le Van C",
                Email = "inspector.c@agriquality.com",
                PasswordHash = "123",
                Role = UserRole.Inspector,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }

}
