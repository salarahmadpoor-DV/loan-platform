using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matchi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Task08ReviewTargetXor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Reviews_Target",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reviews_Target",
                schema: "dbo",
                table: "Reviews",
                sql: "([BusinessId] IS NOT NULL AND [ProviderId] IS NULL) OR ([BusinessId] IS NULL AND [ProviderId] IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Reviews_Target",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reviews_Target",
                schema: "dbo",
                table: "Reviews",
                sql: "[BusinessId] IS NOT NULL OR [ProviderId] IS NOT NULL");
        }
    }
}
