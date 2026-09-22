using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matchi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestLocationForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CityId",
                schema: "dbo",
                table: "RequestLocations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DistrictId",
                schema: "dbo",
                table: "RequestLocations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProvinceId",
                schema: "dbo",
                table: "RequestLocations",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestLocations_CityId",
                schema: "dbo",
                table: "RequestLocations",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestLocations_DistrictId",
                schema: "dbo",
                table: "RequestLocations",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestLocations_ProvinceId",
                schema: "dbo",
                table: "RequestLocations",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestLocations_LocationCities_CityId",
                schema: "dbo",
                table: "RequestLocations",
                column: "CityId",
                principalSchema: "dbo",
                principalTable: "LocationCities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestLocations_LocationDistricts_DistrictId",
                schema: "dbo",
                table: "RequestLocations",
                column: "DistrictId",
                principalSchema: "dbo",
                principalTable: "LocationDistricts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestLocations_LocationProvinces_ProvinceId",
                schema: "dbo",
                table: "RequestLocations",
                column: "ProvinceId",
                principalSchema: "dbo",
                principalTable: "LocationProvinces",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestLocations_LocationCities_CityId",
                schema: "dbo",
                table: "RequestLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestLocations_LocationDistricts_DistrictId",
                schema: "dbo",
                table: "RequestLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestLocations_LocationProvinces_ProvinceId",
                schema: "dbo",
                table: "RequestLocations");

            migrationBuilder.DropIndex(
                name: "IX_RequestLocations_CityId",
                schema: "dbo",
                table: "RequestLocations");

            migrationBuilder.DropIndex(
                name: "IX_RequestLocations_DistrictId",
                schema: "dbo",
                table: "RequestLocations");

            migrationBuilder.DropIndex(
                name: "IX_RequestLocations_ProvinceId",
                schema: "dbo",
                table: "RequestLocations");

            migrationBuilder.DropColumn(
                name: "CityId",
                schema: "dbo",
                table: "RequestLocations");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                schema: "dbo",
                table: "RequestLocations");

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                schema: "dbo",
                table: "RequestLocations");
        }
    }
}
