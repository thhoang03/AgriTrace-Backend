using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.Categories;
using AgriTrace.Domain.Entities.Certificates;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.Notifications;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Entities.Recalls;
using AgriTrace.Domain.Entities.Units;
using AgriTrace.Domain.Entities.Users;
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
        SeedBatches(builder);
        SeedSupplyChainEvents(builder);
        SeedQualityInspections(builder);
        SeedCertificates(builder);
        SeedRecalls(builder);
        SeedNotifications(builder);
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
        // Tất cả seeded users dùng password: Admin@123
        // Hash được tạo bằng User.HashPassword() — PBKDF2/SHA256, 100 000 iterations.
        // Format: {iterations}.{saltBase64}.{keyBase64}
        // Để tạo hash mới: dotnet run --project tools/HashGen -- "YourPassword"
        builder.Entity<UserDataModel>().HasData(
            new UserDataModel
            {
                Id = new Guid("70000000-0000-0000-0000-000000000001"),
                OrganizationId = null,
                FullName = "System Administrator",
                Email = "admin@agritrace.com",
                PasswordHash = "100000.WO50AmM77hFBSqiT1aSFiw==.e1i6MrL9ZZlQF4h2CiK5+qvkR7zilfDmRnLCHfUsNx8=",
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
                PasswordHash = "100000.a67yvmVEWhq7dIjEmejzIg==.8Q3q/IVS35pPn+kp951yFx+MHdVMm6EDdzXB4fqqEL0=",
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
                PasswordHash = "100000.szsbqUNhABlx1s1a8koCTw==.bCSGZ6J7LaqRKz2Jqh55P0VHIdpQHe7+amEZl8Dk62I=",
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
                PasswordHash = "100000.8wke5U2qoW8dhTwYKYXlzQ==.iEDJyugFAUFuNzc5U+3bwcVXt1iNNU/FTZQAzrMwN8I=",
                Role = UserRole.Inspector,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
    private static void SeedBatches(ModelBuilder builder)
    {
        builder.Entity<BatchDataModel>().HasData(
            new BatchDataModel
            {
                Id = new Guid("80000000-0000-0000-0000-000000000001"),
                ProductId = new Guid("60000000-0000-0000-0000-000000000001"),      // Organic Tomato
                CurrentOrganizationId = new Guid("50000000-0000-0000-0000-000000000001"), // Green Farm
                UnitId = new Guid("40000000-0000-0000-0000-000000000001"),         // KG
                BatchCode = "TOMATO-20260105-001",
                ProductionDate = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                ExpiryDate = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                Quantity = 500m,
                RemainingQuantity = 500m,
                SourceQuantity = 500m,
                Status = BatchStatus.Harvested,
                RootBatchId = new Guid("80000000-0000-0000-0000-000000000001"),
                CreatedAt = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc)
            },
            new BatchDataModel
            {
                Id = new Guid("80000000-0000-0000-0000-000000000002"),
                ProductId = new Guid("60000000-0000-0000-0000-000000000002"),      // Dragon Fruit
                CurrentOrganizationId = new Guid("50000000-0000-0000-0000-000000000001"), // Green Farm
                UnitId = new Guid("40000000-0000-0000-0000-000000000004"),         // Milliliter (theo Product seed)
                BatchCode = "DRAGONFRUIT-20260108-001",
                ProductionDate = new DateTime(2026, 1, 8, 0, 0, 0, DateTimeKind.Utc),
                ExpiryDate = new DateTime(2026, 1, 25, 0, 0, 0, DateTimeKind.Utc),
                Quantity = 300m,
                RemainingQuantity = 300m,
                SourceQuantity = 300m,
                Status = BatchStatus.Recalled,
                RootBatchId = new Guid("80000000-0000-0000-0000-000000000002"),
                CreatedAt = new DateTime(2026, 1, 8, 0, 0, 0, DateTimeKind.Utc)
            },
            new BatchDataModel
            {
                Id = new Guid("80000000-0000-0000-0000-000000000003"),
                ProductId = new Guid("60000000-0000-0000-0000-000000000003"),      // Arabica Coffee
                CurrentOrganizationId = new Guid("50000000-0000-0000-0000-000000000002"), // Golden Bean
                UnitId = new Guid("40000000-0000-0000-0000-000000000005"),         // Box
                BatchCode = "COFFEE-20260110-001",
                ProductionDate = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                ExpiryDate = new DateTime(2027, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                Quantity = 200m,
                RemainingQuantity = 150m,
                SourceQuantity = 200m,
                Status = BatchStatus.Transporting,
                RootBatchId = new Guid("80000000-0000-0000-0000-000000000003"),
                CreatedAt = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc)
            },
            new BatchDataModel
            {
                Id = new Guid("80000000-0000-0000-0000-000000000004"),
                ProductId = new Guid("60000000-0000-0000-0000-000000000004"),      // Jasmine Rice
                CurrentOrganizationId = new Guid("50000000-0000-0000-0000-000000000002"), // Golden Bean
                UnitId = new Guid("40000000-0000-0000-0000-000000000002"),         // Gram
                BatchCode = "RICE-20260112-001",
                ProductionDate = new DateTime(2026, 1, 12, 0, 0, 0, DateTimeKind.Utc),
                ExpiryDate = new DateTime(2027, 1, 12, 0, 0, 0, DateTimeKind.Utc),
                Quantity = 1000m,
                RemainingQuantity = 1000m,
                SourceQuantity = 1000m,
                Status = BatchStatus.Recalled,
                RootBatchId = new Guid("80000000-0000-0000-0000-000000000004"),
                CreatedAt = new DateTime(2026, 1, 12, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }

    private static void SeedRecalls(ModelBuilder builder)
    {
        builder.Entity<RecallDataModel>().HasData(
            new RecallDataModel
            {
                Id = new Guid("90000000-0000-0000-0000-000000000001"),
                BatchId = new Guid("80000000-0000-0000-0000-000000000002"),        // Dragon Fruit batch
                CreatedBy = new Guid("70000000-0000-0000-0000-000000000001"),      // Admin
                Reason = "Phát hiện dư lượng thuốc bảo vệ thực vật vượt ngưỡng cho phép.",
                Severity = (int)RecallSeverity.High,
                Status = (int)RecallStatus.Processing,
                CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new RecallDataModel
            {
                Id = new Guid("90000000-0000-0000-0000-000000000002"),
                BatchId = new Guid("80000000-0000-0000-0000-000000000004"),        // Jasmine Rice batch
                CreatedBy = new Guid("70000000-0000-0000-0000-000000000003"),      // Manager (Golden Bean)
                Reason = "Khách hàng phản ánh dị vật lẫn trong bao bì đóng gói.",
                Severity = (int)RecallSeverity.Critical,
                Status = (int)RecallStatus.Pending,
                CreatedAt = new DateTime(2026, 1, 16, 0, 0, 0, DateTimeKind.Utc)
            },
            new RecallDataModel
            {
                Id = new Guid("90000000-0000-0000-0000-000000000003"),
                BatchId = new Guid("80000000-0000-0000-0000-000000000002"),        // Dragon Fruit batch (recall lần 2)
                CreatedBy = new Guid("70000000-0000-0000-0000-000000000001"),      // Admin
                Reason = "Kiểm tra bổ sung sau lần thu hồi trước, lỗi nhẹ về nhãn mác.",
                Severity = (int)RecallSeverity.Low,
                Status = (int)RecallStatus.Completed,
                CreatedAt = new DateTime(2026, 1, 18, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }

    private static void SeedBatches(ModelBuilder builder)
    {
        builder.Entity<BatchDataModel>().HasData(
            new BatchDataModel
            {
                Id = new Guid("80000000-0000-0000-0000-000000000001"),
                ProductId = new Guid("60000000-0000-0000-0000-000000000001"), // Organic Tomato
                CurrentOrganizationId = new Guid("50000000-0000-0000-0000-000000000001"), // Farm
                UnitId = new Guid("40000000-0000-0000-0000-000000000001"), // KG
                BatchCode = "BATCH-TOMATO-001",
                ProductionDate = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                ExpiryDate = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                Quantity = 1000,
                RemainingQuantity = 800,
                SourceQuantity = 1000,
                Status = BatchStatus.Harvested,
                CreatedAt = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new BatchDataModel
            {
                Id = new Guid("80000000-0000-0000-0000-000000000002"),
                ProductId = new Guid("60000000-0000-0000-0000-000000000003"), // Arabica Coffee
                CurrentOrganizationId = new Guid("50000000-0000-0000-0000-000000000002"), // Processor
                UnitId = new Guid("40000000-0000-0000-0000-000000000005"), // Box
                BatchCode = "BATCH-COFFEE-001",
                ProductionDate = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                ExpiryDate = new DateTime(2027, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                Quantity = 500,
                RemainingQuantity = 500,
                SourceQuantity = 500,
                Status = BatchStatus.Processing,
                CreatedAt = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }

    private static void SeedSupplyChainEvents(ModelBuilder builder)
    {
        builder.Entity<SupplyChainEventDataModel>().HasData(
            new SupplyChainEventDataModel
            {
                Id = new Guid("90000000-0000-0000-0000-000000000001"),
                BatchId = new Guid("80000000-0000-0000-0000-000000000001"),
                EventTypeId = new Guid("20000000-0000-0000-0000-000000000001"), // HARVEST
                OrganizationId = new Guid("50000000-0000-0000-0000-000000000001"), // Farm
                PerformedByUserId = new Guid("70000000-0000-0000-0000-000000000002"), // Farmer
                EventData = "Harvested 1000kg of tomatoes",
                Location = "Green Farm Field 1",
                EventTime = new DateTime(2026, 6, 1, 8, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2026, 6, 1, 8, 0, 0, DateTimeKind.Utc)
            },
            new SupplyChainEventDataModel
            {
                Id = new Guid("90000000-0000-0000-0000-000000000002"),
                BatchId = new Guid("80000000-0000-0000-0000-000000000002"),
                EventTypeId = new Guid("20000000-0000-0000-0000-000000000003"), // PROCESSING
                OrganizationId = new Guid("50000000-0000-0000-0000-000000000002"), // Processor
                PerformedByUserId = new Guid("70000000-0000-0000-0000-000000000003"), // Manager
                EventData = "Processed and roasted coffee beans",
                Location = "Golden Bean Factory",
                EventTime = new DateTime(2026, 5, 2, 10, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2026, 5, 2, 10, 0, 0, DateTimeKind.Utc)
            }
        );
    }

    private static void SeedQualityInspections(ModelBuilder builder)
    {
        builder.Entity<QualityInspectionDataModel>().HasData(
            new QualityInspectionDataModel
            {
                Id = new Guid("A0000000-0000-0000-0000-000000000001"),
                BatchId = new Guid("80000000-0000-0000-0000-000000000001"),
                InspectorId = new Guid("70000000-0000-0000-0000-000000000004"), // Inspector
                Status = InspectionStatus.Passed,
                Result = "All standards met. No pesticide residue found.",
                Notes = "Excellent quality.",
                CreatedAt = new DateTime(2026, 6, 2, 9, 0, 0, DateTimeKind.Utc)
            }
        );
    }

    private static void SeedCertificates(ModelBuilder builder)
    {
        builder.Entity<CertificateDataModel>().HasData(
            new CertificateDataModel
            {
                Id = new Guid("B0000000-0000-0000-0000-000000000001"),
                BatchId = new Guid("80000000-0000-0000-0000-000000000001"),
                InspectionId = new Guid("A0000000-0000-0000-0000-000000000001"),
                CertificateType = "Organic Certification",
                FileUrl = "https://agritrace.com/certs/cert-001.pdf",
                IssuedDate = new DateTime(2026, 6, 2, 10, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2026, 6, 2, 10, 0, 0, DateTimeKind.Utc)
            }
        );
    }

    private static void SeedRecalls(ModelBuilder builder)
    {
        builder.Entity<RecallDataModel>().HasData(
            new RecallDataModel
            {
                Id = new Guid("C0000000-0000-0000-0000-000000000001"),
                BatchId = new Guid("80000000-0000-0000-0000-000000000002"),
                CreatedBy = new Guid("70000000-0000-0000-0000-000000000001"), // Admin
                Reason = "Packaging defect",
                Severity = 1,
                Status = 1, // Example status: 1 = Initiated
                CreatedAt = new DateTime(2026, 6, 10, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }

    private static void SeedNotifications(ModelBuilder builder)
    {
        builder.Entity<NotificationDataModel>().HasData(
            new NotificationDataModel
            {
                Id = new Guid("D0000000-0000-0000-0000-000000000001"),
                UserId = new Guid("70000000-0000-0000-0000-000000000002"), // Farmer
                Title = "Harvest Event Recorded",
                Message = "Your harvest event for batch BATCH-TOMATO-001 has been successfully recorded.",
                IsRead = false,
                CreatedAt = new DateTime(2026, 6, 1, 8, 5, 0, DateTimeKind.Utc)
            }
        );
    }
}
