using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Users",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResetPasswordToken",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetPasswordTokenExpiry",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000001"),
                columns: new[] { "Phone", "RefreshToken", "RefreshTokenExpiry", "ResetPasswordToken", "ResetPasswordTokenExpiry" },
                values: new object[] { null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000002"),
                columns: new[] { "Phone", "RefreshToken", "RefreshTokenExpiry", "ResetPasswordToken", "ResetPasswordTokenExpiry" },
                values: new object[] { null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000003"),
                columns: new[] { "Phone", "RefreshToken", "RefreshTokenExpiry", "ResetPasswordToken", "ResetPasswordTokenExpiry" },
                values: new object[] { null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000004"),
                columns: new[] { "Phone", "RefreshToken", "RefreshTokenExpiry", "ResetPasswordToken", "ResetPasswordTokenExpiry" },
                values: new object[] { null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetPasswordToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetPasswordTokenExpiry",
                table: "Users");
        }
    }
}
