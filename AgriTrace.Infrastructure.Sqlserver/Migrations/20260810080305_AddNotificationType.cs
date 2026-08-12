using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "SYSTEM");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000001"),
                column: "Type",
                value: "SYSTEM");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000002"),
                column: "Type",
                value: "SYSTEM");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000003"),
                column: "Type",
                value: "SYSTEM");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000004"),
                column: "Type",
                value: "SYSTEM");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000005"),
                column: "Type",
                value: "SYSTEM");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000006"),
                column: "Type",
                value: "SYSTEM");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000007"),
                column: "Type",
                value: "SYSTEM");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000008"),
                column: "Type",
                value: "SYSTEM");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000001"),
                column: "Type",
                value: "SYSTEM");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");
        }
    }
}
