using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matchi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    /// <summary>
    /// Task 8.2 integrity delta. Not applied to live MatchiDb in this phase.
    /// Concurrency token on ServiceExecution.Status is model-only (no column change).
    /// CK_Reviews_Target XOR is omitted: live Reviews.Id=1 has both BusinessId and ProviderId.
    /// </summary>
    public partial class Task08IntegrityConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Deals_ProposalId",
                schema: "dbo",
                table: "Deals");

            migrationBuilder.CreateIndex(
                name: "UX_ExecutionAssignments_AssignedProvider",
                schema: "dbo",
                table: "ExecutionAssignments",
                columns: new[] { "ServiceExecutionId", "ProviderId" },
                unique: true,
                filter: "[Status] = N'Assigned'");

            migrationBuilder.CreateIndex(
                name: "UX_Deals_ProposalId",
                schema: "dbo",
                table: "Deals",
                column: "ProposalId",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_ExecutionAssignments_AssignedProvider",
                schema: "dbo",
                table: "ExecutionAssignments");

            migrationBuilder.DropIndex(
                name: "UX_Deals_ProposalId",
                schema: "dbo",
                table: "Deals");

            migrationBuilder.CreateIndex(
                name: "UX_Deals_ProposalId",
                schema: "dbo",
                table: "Deals",
                column: "ProposalId",
                unique: true);
        }
    }
}
