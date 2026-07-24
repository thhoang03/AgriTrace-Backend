using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class SeedNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "Message", "Title", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("a0000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 15, 8, 30, 0, 0, DateTimeKind.Utc), "Lô Dragon Fruit (DRAGONFRUIT-20260108-001) đã bị thu hồi do phát hiện dư lượng thuốc bảo vệ thực vật vượt ngưỡng cho phép.", "Cảnh báo thu hồi lô hàng", null, new Guid("70000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "Title", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("a0000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc), true, "Hệ thống AgriTrace đã được khởi tạo thành công với dữ liệu mẫu ban đầu.", "Khởi tạo hệ thống", new DateTime(2026, 1, 1, 9, 15, 0, 0, DateTimeKind.Utc), new Guid("70000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "Message", "Title", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("a0000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 5, 14, 0, 0, 0, DateTimeKind.Utc), "Lô Organic Tomato (TOMATO-20260105-001) đã chuyển sang trạng thái Harvested.", "Cập nhật trạng thái lô hàng", null, new Guid("70000000-0000-0000-0000-000000000002") });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "Title", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("a0000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), true, "Lô Dragon Fruit (DRAGONFRUIT-20260108-001) do bạn cung cấp đã bị thu hồi. Vui lòng kiểm tra chi tiết.", "Lô hàng của bạn bị thu hồi", new DateTime(2026, 1, 16, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("70000000-0000-0000-0000-000000000002") });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "Message", "Title", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 16, 11, 0, 0, 0, DateTimeKind.Utc), "Lô Jasmine Rice (RICE-20260112-001) nhận được phản ánh dị vật lẫn trong bao bì đóng gói từ khách hàng.", "Phản ánh chất lượng sản phẩm", null, new Guid("70000000-0000-0000-0000-000000000003") },
                    { new Guid("a0000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 10, 16, 0, 0, 0, DateTimeKind.Utc), "Lô Arabica Coffee (COFFEE-20260110-001) hiện đang trong trạng thái Transporting, còn lại 150kg.", "Lô hàng đang vận chuyển", null, new Guid("70000000-0000-0000-0000-000000000003") }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "Title", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("a0000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 18, 8, 0, 0, 0, DateTimeKind.Utc), true, "Lô Dragon Fruit (DRAGONFRUIT-20260108-001) cần kiểm tra bổ sung sau lần thu hồi trước liên quan đến lỗi nhãn mác.", "Yêu cầu kiểm tra bổ sung", new DateTime(2026, 1, 18, 8, 20, 0, 0, DateTimeKind.Utc), new Guid("70000000-0000-0000-0000-000000000004") });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "Message", "Title", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("a0000000-0000-0000-0000-000000000008"), new DateTime(2026, 1, 20, 7, 30, 0, 0, DateTimeKind.Utc), "Có 2 lô hàng đang chờ kiểm định chất lượng trong tuần này.", "Lịch kiểm định mới", null, new Guid("70000000-0000-0000-0000-000000000004") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000008"));
        }
    }
}
