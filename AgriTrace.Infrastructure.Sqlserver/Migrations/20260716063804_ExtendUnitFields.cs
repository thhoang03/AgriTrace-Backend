using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class ExtendUnitFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Units",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ConversionToBase",
                table: "Units",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Units",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "Units",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                columns: new[] { "Category", "ConversionToBase", "Description", "Symbol" },
                values: new object[] { 1, 1m, "Metric unit of mass equal to 1000 grams", "kg" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "Category", "ConversionToBase", "Description", "Symbol" },
                values: new object[] { 1, 0.001m, "Metric unit of mass equal to 1/1000 kilogram", "g" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"),
                columns: new[] { "Category", "ConversionToBase", "Description", "Symbol" },
                values: new object[] { 2, 1m, "Metric unit of volume equal to 1000 cubic centimeters", "L" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000004"),
                columns: new[] { "Category", "Code", "ConversionToBase", "Description", "Name", "Symbol" },
                values: new object[] { 2, "MILLILITER", 0.001m, "Metric unit of volume equal to 1/1000 liter", "Milliliter", "mL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000005"),
                columns: new[] { "Category", "Code", "ConversionToBase", "Description", "Name", "Symbol" },
                values: new object[] { 3, "BOX", 1m, "Packaging unit containing a fixed number of items", "Box", "box" });

            migrationBuilder.InsertData(
                table: "Units",
                columns: new[] { "Id", "Category", "Code", "ConversionToBase", "CreatedAt", "Description", "Name", "Symbol", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000006"), 3, "BALE", 1m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Compressed bundle of agricultural produce", "Bale", "bale", null },
                    { new Guid("40000000-0000-0000-0000-000000000007"), 3, "PIECE", 1m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Individual countable item", "Piece", "pc", null },
                    { new Guid("40000000-0000-0000-0000-000000000008"), 1, "TON", 1000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Metric unit of mass equal to 1000 kilograms", "Metric Ton", "t", null },
                    { new Guid("40000000-0000-0000-0000-000000000009"), 1, "SACK", 50m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Standard sack for bulk produce, typically 50 kg", "Sack", "sack", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000009"));

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "ConversionToBase",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "Units");

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000004"),
                columns: new[] { "Code", "Name" },
                values: new object[] { "BOX", "Box" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000005"),
                columns: new[] { "Code", "Name" },
                values: new object[] { "BALE", "Bale" });
        }
    }
}
