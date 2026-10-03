using Matchi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matchi.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(MatchiDbContext))]
    [Migration("20261003113000_AddUserWorkspacePreferences")]
    public class AddUserWorkspacePreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastWorkspace",
                schema: "dbo",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredWorkspace",
                schema: "dbo",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastWorkspace",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PreferredWorkspace",
                schema: "dbo",
                table: "Users");
        }
    }
}
