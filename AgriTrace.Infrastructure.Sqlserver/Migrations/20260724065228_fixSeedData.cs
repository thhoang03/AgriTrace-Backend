using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class fixSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000001"),
                columns: new[] { "Message", "Title" },
                values: new object[] { "Batch Dragon Fruit (DRAGONFRUIT-20260108-001) has been recalled due to pesticide residue exceeding the permitted threshold.", "Batch Recall Alert" });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000002"),
                columns: new[] { "Message", "Title" },
                values: new object[] { "The AgriTrace system has been successfully initialized with initial seed data.", "System Initialized" });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000003"),
                columns: new[] { "Message", "Title" },
                values: new object[] { "Batch Organic Tomato (TOMATO-20260105-001) has transitioned to Harvested status.", "Batch Status Updated" });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000004"),
                columns: new[] { "Message", "Title" },
                values: new object[] { "Batch Dragon Fruit (DRAGONFRUIT-20260108-001) supplied by you has been recalled. Please check the details.", "Your Batch Has Been Recalled" });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000005"),
                columns: new[] { "Message", "Title" },
                values: new object[] { "Batch Jasmine Rice (RICE-20260112-001) received a complaint about foreign objects found inside the packaging from a customer.", "Product Quality Complaint" });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000006"),
                columns: new[] { "Message", "Title" },
                values: new object[] { "Batch Arabica Coffee (COFFEE-20260110-001) is currently in Transporting status with 150 kg remaining.", "Batch In Transit" });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000007"),
                columns: new[] { "Message", "Title" },
                values: new object[] { "Batch Dragon Fruit (DRAGONFRUIT-20260108-001) requires additional inspection following the previous recall related to a labeling defect.", "Additional Inspection Required" });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000008"),
                columns: new[] { "Message", "Title" },
                values: new object[] { "There are 2 batches awaiting quality inspection this week.", "New Inspection Schedule" });

            migrationBuilder.UpdateData(
                table: "Recalls",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000001"),
                column: "Reason",
                value: "Pesticide residue detected exceeding the permitted threshold.");

            migrationBuilder.UpdateData(
                table: "Recalls",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000002"),
                column: "Reason",
                value: "Customer reported foreign objects found inside the packaging.");

            migrationBuilder.UpdateData(
                table: "Recalls",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000003"),
                column: "Reason",
                value: "Follow-up inspection after previous recall; minor labeling defect detected.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000001"),
                columns: new[] { "Message", "Title" },
                values: new object[] { "Lô Dragon Fruit (DRAGONFRUIT-20260108-001) đã bị thu hồi do phát hiện dư lượng thuốc bảo vệ thực vật vượt ngưỡng cho phép.", "Cảnh báo thu hồi lô hàng" });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000002"),
                columns: new[] { "Message", "Title" },
                values: new object[] { "Hệ thống AgriTrace đã được khởi tạo thành công với dữ liệu mẫu ban đầu.", "Khởi tạo hệ thống" });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000003"),
                columns: new[] { "Message", "Title" },
                values: new object[] { "Lô Organic Tomato (TOMATO-20260105-001) đã chuyển sang trạng thái Harvested.", "Cập nhật trạng thái lô hàng" });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000004"),
                columns: new[] { "Message", "Title" },
                values: new object[] { "Lô Dragon Fruit (DRAGONFRUIT-20260108-001) do bạn cung cấp đã bị thu hồi. Vui lòng kiểm tra chi tiết.", "Lô hàng của bạn bị thu hồi" });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000005"),
                columns: new[] { "Message", "Title" },
                values: new object[] { "Lô Jasmine Rice (RICE-20260112-001) nhận được phản ánh dị vật lẫn trong bao bì đóng gói từ khách hàng.", "Phản ánh chất lượng sản phẩm" });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000006"),
                columns: new[] { "Message", "Title" },
                values: new object[] { "Lô Arabica Coffee (COFFEE-20260110-001) hiện đang trong trạng thái Transporting, còn lại 150kg.", "Lô hàng đang vận chuyển" });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000007"),
                columns: new[] { "Message", "Title" },
                values: new object[] { "Lô Dragon Fruit (DRAGONFRUIT-20260108-001) cần kiểm tra bổ sung sau lần thu hồi trước liên quan đến lỗi nhãn mác.", "Yêu cầu kiểm tra bổ sung" });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000008"),
                columns: new[] { "Message", "Title" },
                values: new object[] { "Có 2 lô hàng đang chờ kiểm định chất lượng trong tuần này.", "Lịch kiểm định mới" });

            migrationBuilder.UpdateData(
                table: "Recalls",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000001"),
                column: "Reason",
                value: "Phát hiện dư lượng thuốc bảo vệ thực vật vượt ngưỡng cho phép.");

            migrationBuilder.UpdateData(
                table: "Recalls",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000002"),
                column: "Reason",
                value: "Khách hàng phản ánh dị vật lẫn trong bao bì đóng gói.");

            migrationBuilder.UpdateData(
                table: "Recalls",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000003"),
                column: "Reason",
                value: "Kiểm tra bổ sung sau lần thu hồi trước, lỗi nhẹ về nhãn mác.");
        }
    }
}
