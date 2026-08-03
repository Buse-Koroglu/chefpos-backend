using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefPos.Infastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameCashierIdToCreatedByUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_CashierId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_StockRequests_IngredientId",
                table: "StockRequests");

            migrationBuilder.RenameColumn(
                name: "CashierId",
                table: "Orders",
                newName: "CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CashierId",
                table: "Orders",
                newName: "IX_Orders_CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_CreatedByUserId",
                table: "Orders",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_CreatedByUserId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Orders",
                newName: "CashierId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CreatedByUserId",
                table: "Orders",
                newName: "IX_Orders_CashierId");

            migrationBuilder.CreateIndex(
                name: "IX_StockRequests_IngredientId",
                table: "StockRequests",
                column: "IngredientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_CashierId",
                table: "Orders",
                column: "CashierId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
