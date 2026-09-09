using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matchi.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Task 7 index delta from the stamped MatchiDb baseline.
    /// Does not drop IX_ExecutionAssignments_ServiceExecutionId (it is not on live MatchiDb).
    /// </summary>
    public partial class Task07IndexDelta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceExecutions_DealId",
                schema: "dbo",
                table: "ServiceExecutions");

            migrationBuilder.CreateIndex(
                name: "UX_ServiceExecutions_DealId",
                schema: "dbo",
                table: "ServiceExecutions",
                column: "DealId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Reviews_Deal_Customer_Business",
                schema: "dbo",
                table: "Reviews",
                columns: new[] { "DealId", "CustomerId", "BusinessId" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [BusinessId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Reviews_Deal_Customer_Provider",
                schema: "dbo",
                table: "Reviews",
                columns: new[] { "DealId", "CustomerId", "ProviderId" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [ProviderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionAssignments_ExecutionId_Status",
                schema: "dbo",
                table: "ExecutionAssignments",
                columns: new[] { "ServiceExecutionId", "Status" });

            migrationBuilder.CreateIndex(
                name: "UX_ExecutionAssignments_Primary",
                schema: "dbo",
                table: "ExecutionAssignments",
                column: "ServiceExecutionId",
                unique: true,
                filter: "[IsPrimary] = 1 AND [Status] = N'Assigned'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_ServiceExecutions_DealId",
                schema: "dbo",
                table: "ServiceExecutions");

            migrationBuilder.DropIndex(
                name: "UX_Reviews_Deal_Customer_Business",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "UX_Reviews_Deal_Customer_Provider",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_ExecutionAssignments_ExecutionId_Status",
                schema: "dbo",
                table: "ExecutionAssignments");

            migrationBuilder.DropIndex(
                name: "UX_ExecutionAssignments_Primary",
                schema: "dbo",
                table: "ExecutionAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceExecutions_DealId",
                schema: "dbo",
                table: "ServiceExecutions",
                column: "DealId");
        }
    }
}
