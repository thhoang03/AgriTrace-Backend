using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class RedesignQualityInspection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_QualityInspections_InspectionId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "Result",
                table: "QualityInspections");

            migrationBuilder.AddColumn<DateTime>(
                name: "InspectionDate",
                table: "QualityInspections",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "InspectionType",
                table: "QualityInspections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OverallResult",
                table: "QualityInspections",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InspectionLabTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InspectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MeasuredValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StandardValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionLabTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionLabTests_QualityInspections_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "QualityInspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "QualityInspections",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000001"),
                columns: new[] { "InspectionDate", "InspectionType", "Notes", "OverallResult" },
                values: new object[] { new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Excellent quality. All standards met.", "PASS" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionLabTests_InspectionId",
                table: "InspectionLabTests",
                column: "InspectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_QualityInspections_InspectionId",
                table: "Certificates",
                column: "InspectionId",
                principalTable: "QualityInspections",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_QualityInspections_InspectionId",
                table: "Certificates");

            migrationBuilder.DropTable(
                name: "InspectionLabTests");

            migrationBuilder.DropColumn(
                name: "InspectionDate",
                table: "QualityInspections");

            migrationBuilder.DropColumn(
                name: "InspectionType",
                table: "QualityInspections");

            migrationBuilder.DropColumn(
                name: "OverallResult",
                table: "QualityInspections");

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "QualityInspections",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "QualityInspections",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000001"),
                columns: new[] { "Notes", "Result" },
                values: new object[] { "Excellent quality.", "All standards met. No pesticide residue found." });

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_QualityInspections_InspectionId",
                table: "Certificates",
                column: "InspectionId",
                principalTable: "QualityInspections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
