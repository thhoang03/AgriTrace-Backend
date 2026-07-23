using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedUserPasswordHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000001"),
                column: "PasswordHash",
                value: "100000.WO50AmM77hFBSqiT1aSFiw==.e1i6MrL9ZZlQF4h2CiK5+qvkR7zilfDmRnLCHfUsNx8=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000002"),
                column: "PasswordHash",
                value: "100000.a67yvmVEWhq7dIjEmejzIg==.8Q3q/IVS35pPn+kp951yFx+MHdVMm6EDdzXB4fqqEL0=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000003"),
                column: "PasswordHash",
                value: "100000.szsbqUNhABlx1s1a8koCTw==.bCSGZ6J7LaqRKz2Jqh55P0VHIdpQHe7+amEZl8Dk62I=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000004"),
                column: "PasswordHash",
                value: "100000.8wke5U2qoW8dhTwYKYXlzQ==.iEDJyugFAUFuNzc5U+3bwcVXt1iNNU/FTZQAzrMwN8I=");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000001"),
                column: "PasswordHash",
                value: "123");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000002"),
                column: "PasswordHash",
                value: "123");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000003"),
                column: "PasswordHash",
                value: "123");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000004"),
                column: "PasswordHash",
                value: "123");
        }
    }
}
