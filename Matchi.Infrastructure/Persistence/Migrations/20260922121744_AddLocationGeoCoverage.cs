using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matchi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationGeoCoverage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CenterLat",
                schema: "dbo",
                table: "LocationDistricts",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CenterLng",
                schema: "dbo",
                table: "LocationDistricts",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RadiusKm",
                schema: "dbo",
                table: "LocationDistricts",
                type: "decimal(8,2)",
                precision: 8,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CenterLat",
                schema: "dbo",
                table: "LocationCities",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CenterLng",
                schema: "dbo",
                table: "LocationCities",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RadiusKm",
                schema: "dbo",
                table: "LocationCities",
                type: "decimal(8,2)",
                precision: 8,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CenterLat",
                schema: "dbo",
                table: "LocationDistricts");

            migrationBuilder.DropColumn(
                name: "CenterLng",
                schema: "dbo",
                table: "LocationDistricts");

            migrationBuilder.DropColumn(
                name: "RadiusKm",
                schema: "dbo",
                table: "LocationDistricts");

            migrationBuilder.DropColumn(
                name: "CenterLat",
                schema: "dbo",
                table: "LocationCities");

            migrationBuilder.DropColumn(
                name: "CenterLng",
                schema: "dbo",
                table: "LocationCities");

            migrationBuilder.DropColumn(
                name: "RadiusKm",
                schema: "dbo",
                table: "LocationCities");
        }
    }
}
