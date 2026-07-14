using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class SeedMasterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nhóm rau củ quả tươi", true, "Rau củ", null },
                    { new Guid("30000000-0000-0000-0000-000000000002"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Các loại cà phê", true, "Coffee", null },
                    { new Guid("30000000-0000-0000-0000-000000000003"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gạo các loại", true, "Rice", null },
                    { new Guid("30000000-0000-0000-0000-000000000004"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trái cây tươi", true, "Fruits", null },
                    { new Guid("30000000-0000-0000-0000-000000000005"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Thảo mộc và gia vị", true, "Herbs", null }
                });

            migrationBuilder.InsertData(
                table: "EventTypes",
                columns: new[] { "Id", "Code", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), "HARVEST", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Harvest", null },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "RECEIVE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Receive", null },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "PROCESSING", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Processing", null },
                    { new Guid("20000000-0000-0000-0000-000000000004"), "PACKAGING", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Packaging", null },
                    { new Guid("20000000-0000-0000-0000-000000000005"), "TRANSPORT", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Transport", null },
                    { new Guid("20000000-0000-0000-0000-000000000006"), "DISTRIBUTION", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Distribution", null },
                    { new Guid("20000000-0000-0000-0000-000000000007"), "RETAIL", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Retail", null },
                    { new Guid("20000000-0000-0000-0000-000000000008"), "INSPECTION", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Inspection", null },
                    { new Guid("20000000-0000-0000-0000-000000000009"), "RECALL", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Recall", null }
                });

            migrationBuilder.InsertData(
                table: "OrganizationTypes",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "FARM", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Agricultural Farm", "Farm", null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "PROCESSOR", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Processor", null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "DISTRIBUTOR", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Distributor", null },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "RETAILER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Retailer", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "OrganizationTypes",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "OrganizationTypes",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "OrganizationTypes",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "OrganizationTypes",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"));
        }
    }
}
