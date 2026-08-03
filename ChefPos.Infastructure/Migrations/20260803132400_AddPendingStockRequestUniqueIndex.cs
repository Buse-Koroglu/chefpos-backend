using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefPos.Infastructure.Migrations
{
    public partial class AddPendingStockRequestUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_StockRequests_IngredientId_LocationId",
                table: "StockRequests",
                columns: new[] { "IngredientId", "LocationId" },
                unique: true,
                filter: "\"Status\" = 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StockRequests_IngredientId_LocationId",
                table: "StockRequests");
        }
    }
}
