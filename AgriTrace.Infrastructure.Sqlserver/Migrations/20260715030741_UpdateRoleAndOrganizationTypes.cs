using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoleAndOrganizationTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "OrganizationTypes",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000005"), "INSPECTION", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Inspection", null },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "SYSTEM", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "System", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OrganizationTypes",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "OrganizationTypes",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"));
        }
    }
}
