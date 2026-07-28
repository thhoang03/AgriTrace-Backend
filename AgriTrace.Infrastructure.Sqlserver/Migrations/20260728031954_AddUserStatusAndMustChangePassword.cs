using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class AddUserStatusAndMustChangePassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000001"),
                columns: new[] { "MustChangePassword", "Status" },
                values: new object[] { false, "Active" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000002"),
                columns: new[] { "MustChangePassword", "Status" },
                values: new object[] { false, "Active" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000003"),
                columns: new[] { "MustChangePassword", "Status" },
                values: new object[] { false, "Active" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000004"),
                columns: new[] { "MustChangePassword", "Status" },
                values: new object[] { false, "Active" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000005"),
                columns: new[] { "MustChangePassword", "Status" },
                values: new object[] { false, "Active" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000006"),
                columns: new[] { "MustChangePassword", "Status" },
                values: new object[] { false, "Active" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000009"),
                columns: new[] { "MustChangePassword", "Status" },
                values: new object[] { false, "Active" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000a"),
                columns: new[] { "MustChangePassword", "Status" },
                values: new object[] { false, "Active" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000b"),
                columns: new[] { "MustChangePassword", "Status" },
                values: new object[] { false, "Active" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000c"),
                columns: new[] { "MustChangePassword", "Status" },
                values: new object[] { false, "Active" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000d"),
                columns: new[] { "MustChangePassword", "Status" },
                values: new object[] { false, "Active" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");
        }
    }
}
