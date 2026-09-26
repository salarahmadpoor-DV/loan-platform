using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matchi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationProvinces",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationProvinces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocationCities",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProvinceId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationCities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationCities_LocationProvinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalSchema: "dbo",
                        principalTable: "LocationProvinces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LocationDistricts",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationDistricts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationDistricts_LocationCities_CityId",
                        column: x => x.CityId,
                        principalSchema: "dbo",
                        principalTable: "LocationCities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationCities_ProvinceId",
                schema: "dbo",
                table: "LocationCities",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationDistricts_CityId",
                schema: "dbo",
                table: "LocationDistricts",
                column: "CityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationDistricts",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LocationCities",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LocationProvinces",
                schema: "dbo");
        }
    }
}
