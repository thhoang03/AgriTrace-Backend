using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Batches",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000002"),
                column: "UnitId",
                value: new Guid("40000000-0000-0000-0000-000000000001"));

            migrationBuilder.UpdateData(
                table: "Batches",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000004"),
                columns: new[] { "Quantity", "RemainingQuantity", "SourceQuantity", "UnitId" },
                values: new object[] { 20m, 20m, 20m, new Guid("40000000-0000-0000-0000-000000000009") });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"),
                column: "UnitId",
                value: new Guid("40000000-0000-0000-0000-000000000001"));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000004"),
                column: "UnitId",
                value: new Guid("40000000-0000-0000-0000-000000000009"));

            migrationBuilder.UpdateData(
                table: "Recalls",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000002"),
                column: "Status",
                value: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Batches",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000002"),
                column: "UnitId",
                value: new Guid("40000000-0000-0000-0000-000000000004"));

            migrationBuilder.UpdateData(
                table: "Batches",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000004"),
                columns: new[] { "Quantity", "RemainingQuantity", "SourceQuantity", "UnitId" },
                values: new object[] { 1000m, 1000m, 1000m, new Guid("40000000-0000-0000-0000-000000000002") });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"),
                column: "UnitId",
                value: new Guid("40000000-0000-0000-0000-000000000004"));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000004"),
                column: "UnitId",
                value: new Guid("40000000-0000-0000-0000-000000000002"));

            migrationBuilder.UpdateData(
                table: "Recalls",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000002"),
                column: "Status",
                value: 1);
        }
    }
}