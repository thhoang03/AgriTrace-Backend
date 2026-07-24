using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class ExpandSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EventTypes",
                columns: new[] { "Id", "Code", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-00000000000a"), "SPLIT", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Split", null },
                    { new Guid("20000000-0000-0000-0000-00000000000b"), "MERGE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Merge", null }
                });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "Address", "CreatedAt", "Name", "OrganizationTypeId", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000005"), "District 1, Ho Chi Minh City", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fresh Market Retailer", new Guid("10000000-0000-0000-0000-000000000004"), 1, null },
                    { new Guid("50000000-0000-0000-0000-000000000006"), "Hanoi City", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System Operator", new Guid("10000000-0000-0000-0000-000000000006"), 1, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "OrganizationId", "PasswordHash", "Phone", "RefreshToken", "RefreshTokenExpiry", "ResetPasswordToken", "ResetPasswordTokenExpiry", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("70000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff.d@freshlink.com", "Pham Van D", true, new Guid("50000000-0000-0000-0000-000000000003"), "100000.abc123def456ghi789jkl012mno345pqr678stu901vwx234yzA5678==", null, null, null, null, null, "Staff", null },
                    { new Guid("70000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager.e@freshmarket.com", "Nguyen Thi E", true, new Guid("50000000-0000-0000-0000-000000000005"), "100000.xyz789abc012def345ghi678jkl901mno234pqr567stu890vwx123yzA4567==", null, null, null, null, null, "Manager", null },
                    { new Guid("70000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system@agritrace.com", "System Operator", true, new Guid("50000000-0000-0000-0000-000000000006"), "100000.sys0000ops0000sys0000ops0000sys0000ops0000sys0000ops0000==", null, null, null, null, null, "Admin", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-00000000000a"));

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-00000000000b"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000006"));
        }
    }
}
