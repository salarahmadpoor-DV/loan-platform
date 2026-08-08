using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matchi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanRequest_Banks_BankId",
                table: "LoanRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoanRequest",
                table: "LoanRequest");


            migrationBuilder.RenameIndex(
                name: "IX_LoanRequest_BankId",
                table: "LoanRequest",
                newName: "IX_LoanRequests_BankId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoanRequests",
                table: "LoanRequest",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanRequests_Banks_BankId",
                table: "LoanRequest",
                column: "BankId",
                principalSchema: "dbo",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanRequests_Banks_BankId",
                table: "LoanRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoanRequest",
                table: "LoanRequest");

            migrationBuilder.RenameIndex(
                name: "IX_LoanRequests_BankId",
                table: "LoanRequest",
                newName: "IX_LoanRequest_BankId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoanRequest",
                table: "LoanReques",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanRequest_Banks_BankId",
                table: "LoanRequest",
                column: "BankId",
                principalSchema: "Loan",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
