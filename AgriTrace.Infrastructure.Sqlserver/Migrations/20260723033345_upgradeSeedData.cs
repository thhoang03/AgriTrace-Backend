using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class upgradeSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "Message", "Title", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("d0000000-0000-0000-0000-000000000001"), new DateTime(2026, 6, 1, 8, 5, 0, 0, DateTimeKind.Utc), "Your harvest event for batch BATCH-TOMATO-001 has been successfully recorded.", "Harvest Event Recorded", null, new Guid("70000000-0000-0000-0000-000000000002") });

            migrationBuilder.InsertData(
                table: "QualityInspections",
                columns: new[] { "Id", "BatchId", "CreatedAt", "InspectorId", "Notes", "Result", "Status", "UpdatedAt" },
                values: new object[] { new Guid("a0000000-0000-0000-0000-000000000001"), new Guid("80000000-0000-0000-0000-000000000001"), new DateTime(2026, 6, 2, 9, 0, 0, 0, DateTimeKind.Utc), new Guid("70000000-0000-0000-0000-000000000004"), "Excellent quality.", "All standards met. No pesticide residue found.", 2, null });

            migrationBuilder.InsertData(
                table: "SupplyChainEvents",
                columns: new[] { "Id", "BatchId", "CreatedAt", "CurrentHash", "EventData", "EventTime", "EventTypeId", "Location", "OrganizationId", "PerformedByUserId", "PreviousHash", "UpdatedAt", "UserDataModelId" },
                values: new object[,]
                {
                    { new Guid("90000000-0000-0000-0000-000000000001"), new Guid("80000000-0000-0000-0000-000000000001"), new DateTime(2026, 6, 1, 8, 0, 0, 0, DateTimeKind.Utc), null, "Harvested 1000kg of tomatoes", new DateTime(2026, 6, 1, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("20000000-0000-0000-0000-000000000001"), "Green Farm Field 1", new Guid("50000000-0000-0000-0000-000000000001"), new Guid("70000000-0000-0000-0000-000000000002"), null, null, null },
                    { new Guid("90000000-0000-0000-0000-000000000002"), new Guid("80000000-0000-0000-0000-000000000002"), new DateTime(2026, 5, 2, 10, 0, 0, 0, DateTimeKind.Utc), null, "Processed and roasted coffee beans", new DateTime(2026, 5, 2, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("20000000-0000-0000-0000-000000000003"), "Golden Bean Factory", new Guid("50000000-0000-0000-0000-000000000002"), new Guid("70000000-0000-0000-0000-000000000003"), null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Certificates",
                columns: new[] { "Id", "BatchId", "CertificateType", "CreatedAt", "FileUrl", "InspectionId", "IssuedDate", "UpdatedAt" },
                values: new object[] { new Guid("b0000000-0000-0000-0000-000000000001"), new Guid("80000000-0000-0000-0000-000000000001"), "Organic Certification", new DateTime(2026, 6, 2, 10, 0, 0, 0, DateTimeKind.Utc), "https://agritrace.com/certs/cert-001.pdf", new Guid("a0000000-0000-0000-0000-000000000001"), new DateTime(2026, 6, 2, 10, 0, 0, 0, DateTimeKind.Utc), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Certificates",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "SupplyChainEvents",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "SupplyChainEvents",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "QualityInspections",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000001"));
        }
    }
}
