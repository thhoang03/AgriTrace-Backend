using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class updateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Fresh vegetables and tubers", "Vegetables" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                column: "Description",
                value: "Various types of coffee");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                column: "Description",
                value: "Various types of rice");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000004"),
                column: "Description",
                value: "Fresh fruits");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000005"),
                column: "Description",
                value: "Herbs and spices");

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "Address", "CreatedAt", "Name", "OrganizationTypeId", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000001"), "Tan Lac, Hoa Binh Province", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Green Farm Co.", new Guid("10000000-0000-0000-0000-000000000001"), 1, null },
                    { new Guid("50000000-0000-0000-0000-000000000002"), "Buon Ma Thuot, Dak Lak Province", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Golden Bean Processor", new Guid("10000000-0000-0000-0000-000000000002"), 1, null },
                    { new Guid("50000000-0000-0000-0000-000000000003"), "Binh Tan, Ho Chi Minh City", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fresh Link Distributor", new Guid("10000000-0000-0000-0000-000000000003"), 1, null },
                    { new Guid("50000000-0000-0000-0000-000000000004"), "Cau Giay, Hanoi City", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Agri Quality Inspection", new Guid("10000000-0000-0000-0000-000000000005"), 1, null }
                });

            migrationBuilder.InsertData(
                table: "Units",
                columns: new[] { "Id", "Code", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), "KG", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kilogram", null },
                    { new Guid("40000000-0000-0000-0000-000000000002"), "GRAM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gram", null },
                    { new Guid("40000000-0000-0000-0000-000000000003"), "LITER", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Liter", null },
                    { new Guid("40000000-0000-0000-0000-000000000004"), "BOX", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Box", null },
                    { new Guid("40000000-0000-0000-0000-000000000005"), "BALE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bale", null }
                });

            migrationBuilder.Sql(@"
INSERT INTO [Users] ([Id], [CreatedAt], [Email], [FullName], [IsActive], [OrganizationId], [PasswordHash], [Role], [UpdatedAt])
VALUES ('70000000-0000-0000-0000-000000000001', '2026-01-01T00:00:00.0000000Z', N'admin@agritrace.com', N'System Administrator', CAST(1 AS bit), NULL, N'100000.WO50AmM77hFBSqiT1aSFiw==.e1i6MrL9ZZlQF4h2CiK5+qvkR7zilfDmRnLCHfUsNx8=', CAST(1 AS int), NULL);
");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Name", "OrganizationId", "UnitId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("60000000-0000-0000-0000-000000000001"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Organic Tomato", new Guid("50000000-0000-0000-0000-000000000001"), new Guid("40000000-0000-0000-0000-000000000001"), null },
                    { new Guid("60000000-0000-0000-0000-000000000002"), new Guid("30000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dragon Fruit", new Guid("50000000-0000-0000-0000-000000000001"), new Guid("40000000-0000-0000-0000-000000000004"), null },
                    { new Guid("60000000-0000-0000-0000-000000000003"), new Guid("30000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Arabica Coffee", new Guid("50000000-0000-0000-0000-000000000002"), new Guid("40000000-0000-0000-0000-000000000005"), null },
                    { new Guid("60000000-0000-0000-0000-000000000004"), new Guid("30000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Jasmine Rice", new Guid("50000000-0000-0000-0000-000000000002"), new Guid("40000000-0000-0000-0000-000000000002"), null }
                });

            migrationBuilder.Sql(@"
INSERT INTO [Users] ([Id], [CreatedAt], [Email], [FullName], [IsActive], [OrganizationId], [PasswordHash], [Role], [UpdatedAt])
VALUES 
('70000000-0000-0000-0000-000000000002', '2026-01-01T00:00:00.0000000Z', N'farmer.a@greenfarm.com', N'Nguyen Van A', CAST(1 AS bit), '50000000-0000-0000-0000-000000000001', N'100000.a67yvmVEWhq7dIjEmejzIg==.8Q3q/IVS35pPn+kp951yFx+MHdVMm6EDdzXB4fqqEL0=', CAST(3 AS int), NULL),
('70000000-0000-0000-0000-000000000003', '2026-01-01T00:00:00.0000000Z', N'manager.b@goldenbean.com', N'Tran Thi B', CAST(1 AS bit), '50000000-0000-0000-0000-000000000002', N'100000.szsbqUNhABlx1s1a8koCTw==.bCSGZ6J7LaqRKz2Jqh55P0VHIdpQHe7+amEZl8Dk62I=', CAST(2 AS int), NULL),
('70000000-0000-0000-0000-000000000004', '2026-01-01T00:00:00.0000000Z', N'inspector.c@agriquality.com', N'Le Van C', CAST(1 AS bit), '50000000-0000-0000-0000-000000000004', N'100000.8wke5U2qoW8dhTwYKYXlzQ==.iEDJyugFAUFuNzc5U+3bwcVXt1iNNU/FTZQAzrMwN8I=', CAST(5 AS int), NULL);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000005"));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Fresh vegetables and tubers", "Vegetables" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                column: "Description",
                value: "Various types of coffee");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                column: "Description",
                value: "Various types of rice");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000004"),
                column: "Description",
                value: "Fresh fruits");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000005"),
                column: "Description",
                value: "Herbs and spices");

            // Set Vietnamese collation for DB and relevant columns
            migrationBuilder.Sql(
                "ALTER DATABASE [AgriTraceDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;",
                suppressTransaction: true);
            migrationBuilder.Sql(
                "ALTER DATABASE [AgriTraceDB] COLLATE Vietnamese_CI_AS;",
                suppressTransaction: true);
            migrationBuilder.Sql(
                "ALTER DATABASE [AgriTraceDB] SET MULTI_USER;",
                suppressTransaction: true);

            migrationBuilder.Sql("ALTER TABLE [Categories] ALTER COLUMN [Name] NVARCHAR(100) COLLATE Vietnamese_CI_AS;");
            migrationBuilder.Sql("ALTER TABLE [Categories] ALTER COLUMN [Description] NVARCHAR(500) COLLATE Vietnamese_CI_AS;");
            migrationBuilder.Sql("ALTER TABLE [Organizations] ALTER COLUMN [Name] NVARCHAR(200) COLLATE Vietnamese_CI_AS;");
            migrationBuilder.Sql("ALTER TABLE [Organizations] ALTER COLUMN [Address] NVARCHAR(500) COLLATE Vietnamese_CI_AS;");
            migrationBuilder.Sql("ALTER TABLE [Products] ALTER COLUMN [Name] NVARCHAR(200) COLLATE Vietnamese_CI_AS;");
            migrationBuilder.Sql("ALTER TABLE [Users] ALTER COLUMN [FullName] NVARCHAR(MAX) COLLATE Vietnamese_CI_AS;");
            migrationBuilder.Sql("ALTER TABLE [Users] ALTER COLUMN [PasswordHash] NVARCHAR(MAX) COLLATE Vietnamese_CI_AS;");
        }
    }
}
