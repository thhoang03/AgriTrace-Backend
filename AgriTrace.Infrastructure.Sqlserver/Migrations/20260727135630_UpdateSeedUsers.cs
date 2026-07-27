using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000e"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "OrganizationId", "PasswordHash", "Phone", "RefreshToken", "RefreshTokenExpiry", "ResetPasswordToken", "ResetPasswordTokenExpiry", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("70000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager.f@systemop.com", "System Operator", true, new Guid("50000000-0000-0000-0000-000000000006"), "100000.ZCEfWZ2DeDBafl7sSoMR+w==.vS5N2A5Y2xxC3dQylLJes39s1xHrhN/mNbLz5D1/KVo=", null, null, null, null, null, "Manager", null },
                    { new Guid("70000000-0000-0000-0000-00000000000e"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff.f@systemop.com", "System Staff", true, new Guid("50000000-0000-0000-0000-000000000006"), "100000.jlzfdeBqq5BkzvHyVkVUkw==.R3KvBcnhxzGqNbNFgJG3HPc1ozb7L+8OUUHjIUQAgOw=", null, null, null, null, null, "Staff", null }
                });
        }
    }
}
