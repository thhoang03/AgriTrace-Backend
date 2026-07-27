using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class MigrateObsoleteRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000002"),
                columns: new[] { "Email", "Role" },
                values: new object[] { "staff.a@greenfarm.com", "Staff" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000004"),
                columns: new[] { "Email", "Role" },
                values: new object[] { "staff.c@agriquality.com", "Staff" });

            migrationBuilder.Sql(
                "UPDATE Users SET Role = 'Staff' WHERE Role IN ('Farmer', 'Inspector', 'Consumer')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000002"),
                columns: new[] { "Email", "Role" },
                values: new object[] { "farmer.a@greenfarm.com", "Farmer" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000004"),
                columns: new[] { "Email", "Role" },
                values: new object[] { "inspector.c@agriquality.com", "Inspector" });

            migrationBuilder.Sql(
                "UPDATE Users SET Role = 'Farmer' WHERE Email = 'staff.a@greenfarm.com'");
            migrationBuilder.Sql(
                "UPDATE Users SET Role = 'Inspector' WHERE Email = 'staff.c@agriquality.com'");
        }
    }
}
