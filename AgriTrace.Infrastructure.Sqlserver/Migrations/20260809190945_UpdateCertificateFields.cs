using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCertificateFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CertificateNumber",
                table: "Certificates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "Certificates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssuingOrganization",
                table: "Certificates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Certificates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Certificates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Certificates",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000001"),
                columns: new[] { "CertificateNumber", "CreatedAt", "ExpiryDate", "FileUrl", "IssuedDate", "IssuingOrganization", "Notes", "Status" },
                values: new object[] { "ORG-VN-2026-001", new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2027, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "https://agritrace.com/certs/cert-organic-001.pdf", new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Control Union Certifications", null, 3 });

            migrationBuilder.InsertData(
                table: "Certificates",
                columns: new[] { "Id", "BatchId", "CertificateNumber", "CertificateType", "CreatedAt", "ExpiryDate", "FileUrl", "InspectionId", "IssuedDate", "IssuingOrganization", "Notes", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("b0000000-0000-0000-0000-000000000003"), new Guid("80000000-0000-0000-0000-000000000003"), "GG-2026-992", "GlobalG.A.P. Certificate", new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2027, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "https://agritrace.com/certs/cert-globalgap-coffee.pdf", null, new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Eurofins Global Standards", null, 3, null },
                    { new Guid("b0000000-0000-0000-0000-000000000004"), new Guid("80000000-0000-0000-0000-000000000004"), "VG-2026-8819", "VietGAP Certificate", new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2027, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), "https://agritrace.com/certs/cert-vietgap-rice.pdf", null, new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Ministry of Agriculture & RD", null, 3, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Certificates",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Certificates",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000004"));

            migrationBuilder.DropColumn(
                name: "CertificateNumber",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "IssuingOrganization",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Certificates");

            migrationBuilder.UpdateData(
                table: "Certificates",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "FileUrl", "IssuedDate" },
                values: new object[] { new DateTime(2026, 6, 2, 10, 0, 0, 0, DateTimeKind.Utc), "https://agritrace.com/certs/cert-001.pdf", new DateTime(2026, 6, 2, 10, 0, 0, 0, DateTimeKind.Utc) });
        }
    }
}
