using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class EnsureOneManagerAndStaffPerOrg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000007"),
                columns: new[] { "Email", "Role" },
                values: new object[] { "manager.f@systemop.com", "Manager" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "OrganizationId", "PasswordHash", "Phone", "RefreshToken", "RefreshTokenExpiry", "ResetPasswordToken", "ResetPasswordTokenExpiry", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("70000000-0000-0000-0000-000000000009"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager.a@greenfarm.com", "Tran Van A", true, new Guid("50000000-0000-0000-0000-000000000001"), "100000.mgrA1000farm0000mgrA1000farm0000mgrA1000farm0000==", null, null, null, null, null, "Manager", null },
                    { new Guid("70000000-0000-0000-0000-00000000000a"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff.b@goldenbean.com", "Le Van B", true, new Guid("50000000-0000-0000-0000-000000000002"), "100000.stfB2000bean0000stfB2000bean0000stfB2000bean0000==", null, null, null, null, null, "Staff", null },
                    { new Guid("70000000-0000-0000-0000-00000000000b"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager.d@freshlink.com", "Hoang Van E", true, new Guid("50000000-0000-0000-0000-000000000003"), "100000.mgrD4000link0000mgrD4000link0000mgrD4000link0000==", null, null, null, null, null, "Manager", null },
                    { new Guid("70000000-0000-0000-0000-00000000000c"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager.c@agriquality.com", "Pham Thi D", true, new Guid("50000000-0000-0000-0000-000000000004"), "100000.mgrC3000qual0000mgrC3000qual0000mgrC3000qual0000==", null, null, null, null, null, "Manager", null },
                    { new Guid("70000000-0000-0000-0000-00000000000d"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff.e@freshmarket.com", "Tran Thi F", true, new Guid("50000000-0000-0000-0000-000000000005"), "100000.stfE5000mart0000stfE5000mart0000stfE5000mart0000==", null, null, null, null, null, "Staff", null },
                    { new Guid("70000000-0000-0000-0000-00000000000e"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff.f@systemop.com", "System Staff", true, new Guid("50000000-0000-0000-0000-000000000006"), "100000.stfF6000sys0000stfF6000sys0000stfF6000sys0000==", null, null, null, null, null, "Staff", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000a"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000b"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000c"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000d"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000e"));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000007"),
                columns: new[] { "Email", "Role" },
                values: new object[] { "system@agritrace.com", "Admin" });
        }
    }
}
