using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueueManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class f1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Branches_BranchId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_BranchId_ServiceName",
                table: "Services");

            migrationBuilder.CreateIndex(
                name: "IX_Services_BranchId_ServiceName",
                table: "Services",
                columns: new[] { "BranchId", "ServiceName" },
                unique: true,
                filter: "[BranchId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Branches_BranchId",
                table: "Services",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Branches_BranchId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_BranchId_ServiceName",
                table: "Services");

            migrationBuilder.CreateIndex(
                name: "IX_Services_BranchId_ServiceName",
                table: "Services",
                columns: new[] { "BranchId", "ServiceName" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Branches_BranchId",
                table: "Services",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
