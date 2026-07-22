using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class SeedBatchesAndRecalls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Batches",
                columns: new[] { "Id", "BatchCode", "CreatedAt", "CurrentOrganizationId", "ExpiryDate", "OrganizationDataModelId", "ParentBatchId", "ProductId", "ProductionDate", "QRCode", "Quantity", "RemainingQuantity", "RootBatchId", "SourceQuantity", "SplitId", "Status", "UnitId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("80000000-0000-0000-0000-000000000001"), "TOMATO-20260105-001", new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("50000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("60000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, 500m, 500m, new Guid("80000000-0000-0000-0000-000000000001"), 500m, null, 2, new Guid("40000000-0000-0000-0000-000000000001"), null },
                    { new Guid("80000000-0000-0000-0000-000000000002"), "DRAGONFRUIT-20260108-001", new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("50000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("60000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, 300m, 300m, new Guid("80000000-0000-0000-0000-000000000002"), 300m, null, 7, new Guid("40000000-0000-0000-0000-000000000004"), null },
                    { new Guid("80000000-0000-0000-0000-000000000003"), "COFFEE-20260110-001", new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("50000000-0000-0000-0000-000000000002"), new DateTime(2027, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("60000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, 200m, 150m, new Guid("80000000-0000-0000-0000-000000000003"), 200m, null, 4, new Guid("40000000-0000-0000-0000-000000000005"), null },
                    { new Guid("80000000-0000-0000-0000-000000000004"), "RICE-20260112-001", new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("50000000-0000-0000-0000-000000000002"), new DateTime(2027, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("60000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, 1000m, 1000m, new Guid("80000000-0000-0000-0000-000000000004"), 1000m, null, 7, new Guid("40000000-0000-0000-0000-000000000002"), null }
                });

            migrationBuilder.InsertData(
                table: "Recalls",
                columns: new[] { "Id", "BatchId", "CreatedAt", "CreatedBy", "Reason", "RecallDataModelId", "Severity", "Status", "UpdatedAt", "UserDataModelId" },
                values: new object[,]
                {
                    { new Guid("90000000-0000-0000-0000-000000000001"), new Guid("80000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("70000000-0000-0000-0000-000000000001"), "Phát hiện dư lượng thuốc bảo vệ thực vật vượt ngưỡng cho phép.", null, 3, 2, null, null },
                    { new Guid("90000000-0000-0000-0000-000000000002"), new Guid("80000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("70000000-0000-0000-0000-000000000003"), "Khách hàng phản ánh dị vật lẫn trong bao bì đóng gói.", null, 4, 1, null, null },
                    { new Guid("90000000-0000-0000-0000-000000000003"), new Guid("80000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 18, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("70000000-0000-0000-0000-000000000001"), "Kiểm tra bổ sung sau lần thu hồi trước, lỗi nhẹ về nhãn mác.", null, 1, 3, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Batches",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Batches",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Recalls",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Recalls",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Recalls",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Batches",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Batches",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000004"));
        }
    }
}
