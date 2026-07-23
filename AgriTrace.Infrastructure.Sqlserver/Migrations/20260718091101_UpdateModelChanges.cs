using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Units_UnitId",
                table: "Batches");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<decimal>(
                name: "SourceQuantity",
                table: "Batches",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "SplitId",
                table: "Batches",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000001"),
                column: "Role",
                value: "Admin");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000002"),
                column: "Role",
                value: "Farmer");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000003"),
                column: "Role",
                value: "Manager");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000004"),
                column: "Role",
                value: "Inspector");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_QRCode",
                table: "Batches",
                column: "QRCode");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_RootBatchId",
                table: "Batches",
                column: "RootBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_SplitId",
                table: "Batches",
                column: "SplitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Units_UnitId",
                table: "Batches",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Units_UnitId",
                table: "Batches");

            migrationBuilder.DropIndex(
                name: "IX_Batches_QRCode",
                table: "Batches");

            migrationBuilder.DropIndex(
                name: "IX_Batches_RootBatchId",
                table: "Batches");

            migrationBuilder.DropIndex(
                name: "IX_Batches_SplitId",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "SourceQuantity",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "SplitId",
                table: "Batches");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Users",
                type: "int",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000001"),
                column: "Role",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000002"),
                column: "Role",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000003"),
                column: "Role",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000004"),
                column: "Role",
                value: 5);

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Units_UnitId",
                table: "Batches",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
