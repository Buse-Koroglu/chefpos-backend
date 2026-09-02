using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefPos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientAndRefactorProductItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ProductItem_UnitPrice_NonNegative",
                table: "ProductItems");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProductItems");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "ProductItems");

            migrationBuilder.AddColumn<Guid>(
                name: "IngredientId",
                table: "ProductItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "QuantityPerServing",
                table: "ProductItems",
                type: "numeric(10,3)",
                precision: 10,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CurrentStock = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: false),
                    MinStockThreshold = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.Id);
                    table.CheckConstraint("CK_Ingredient_CurrentStock_NonNegative", "\"CurrentStock\" >= 0");
                    table.CheckConstraint("CK_Ingredient_MinStockThreshold_NonNegative", "\"MinStockThreshold\" >= 0");
                    table.CheckConstraint("CK_Ingredient_UnitPrice_NonNegative", "\"UnitPrice\" >= 0");
                    table.ForeignKey(
                        name: "FK_Ingredients_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductItems_IngredientId",
                table: "ProductItems",
                column: "IngredientId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ProductItem_QuantityPerServing_Positive",
                table: "ProductItems",
                sql: "\"QuantityPerServing\" > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_LocationId_Name",
                table: "Ingredients",
                columns: new[] { "LocationId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductItems_Ingredients_IngredientId",
                table: "ProductItems",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductItems_Ingredients_IngredientId",
                table: "ProductItems");

            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_ProductItems_IngredientId",
                table: "ProductItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ProductItem_QuantityPerServing_Positive",
                table: "ProductItems");

            migrationBuilder.DropColumn(
                name: "IngredientId",
                table: "ProductItems");

            migrationBuilder.DropColumn(
                name: "QuantityPerServing",
                table: "ProductItems");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProductItems",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "ProductItems",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddCheckConstraint(
                name: "CK_ProductItem_UnitPrice_NonNegative",
                table: "ProductItems",
                sql: "\"UnitPrice\" >= 0");
        }
    }
}
