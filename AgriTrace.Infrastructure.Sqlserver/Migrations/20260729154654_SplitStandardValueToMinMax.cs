using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class SplitStandardValueToMinMax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StandardValue",
                table: "InspectionLabTests",
                newName: "MinStandardValue");

            migrationBuilder.AddColumn<string>(
                name: "MaxStandardValue",
                table: "InspectionLabTests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxStandardValue",
                table: "InspectionLabTests");

            migrationBuilder.RenameColumn(
                name: "MinStandardValue",
                table: "InspectionLabTests",
                newName: "StandardValue");
        }
    }
}
