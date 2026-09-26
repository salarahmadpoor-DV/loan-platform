using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matchi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationCoverageCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "dbo",
                table: "LocationCities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "dbo",
                table: "LocationDistricts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "UX_LocationCities_Code",
                schema: "dbo",
                table: "LocationCities",
                column: "Code",
                unique: true,
                filter: "([Code]<>N'')");

            migrationBuilder.CreateIndex(
                name: "UX_LocationDistricts_Code",
                schema: "dbo",
                table: "LocationDistricts",
                column: "Code",
                unique: true,
                filter: "([Code]<>N'')");

            migrationBuilder.CreateIndex(
                name: "UX_LocationProvinces_Code",
                schema: "dbo",
                table: "LocationProvinces",
                column: "Code",
                unique: true,
                filter: "([Code]<>N'')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_LocationCities_Code",
                schema: "dbo",
                table: "LocationCities");

            migrationBuilder.DropIndex(
                name: "UX_LocationDistricts_Code",
                schema: "dbo",
                table: "LocationDistricts");

            migrationBuilder.DropIndex(
                name: "UX_LocationProvinces_Code",
                schema: "dbo",
                table: "LocationProvinces");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "dbo",
                table: "LocationCities");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "dbo",
                table: "LocationDistricts");
        }
    }
}
