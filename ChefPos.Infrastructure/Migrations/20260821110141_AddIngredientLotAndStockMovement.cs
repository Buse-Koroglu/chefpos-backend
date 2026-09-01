using Microsoft.EntityFrameworkCore.Migrations;

namespace ChefPos.Infastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientLotAndStockMovement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IngredientLots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uuid", nullable: false),
                    InitialQuantity = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: false),
                    RemainingQuantity = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SourceStockRequestId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientLots", x => x.Id);
                    table.CheckConstraint("CK_IngredientLot_InitialQuantity_Positive", "\"InitialQuantity\" > 0");
                    table.CheckConstraint("CK_IngredientLot_RemainingQuantity_NonNegative", "\"RemainingQuantity\" >= 0");
                    table.CheckConstraint("CK_IngredientLot_UnitPrice_NonNegative", "\"UnitPrice\" >= 0");
                    table.ForeignKey(
                        name: "FK_IngredientLots_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(@"
                CREATE EXTENSION IF NOT EXISTS pgcrypto;
                INSERT INTO ""IngredientLots""
                    (""Id"", ""IngredientId"", ""InitialQuantity"", ""RemainingQuantity"", ""UnitPrice"", ""PurchasedAt"", ""SourceStockRequestId"", ""CreatedAt"", ""UpdatedAt"")
                SELECT
                    gen_random_uuid(),
                    ""Id"",
                    ""CurrentStock"",
                    ""CurrentStock"",
                    ""UnitPrice"",
                    ""CreatedAt"",
                    NULL,
                    now(),
                    NULL
                FROM ""Ingredients""
                WHERE ""CurrentStock"" > 0;
            ");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Ingredient_UnitPrice_NonNegative",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "Ingredients");

            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: false),
                    PerformedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RelatedOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    RelatedProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovements", x => x.Id);
                    table.CheckConstraint("CK_StockMovement_Quantity_Positive", "\"Quantity\" > 0");
                    table.ForeignKey(
                        name: "FK_StockMovements_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Users_PerformedByUserId",
                        column: x => x.PerformedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockMovementLotConsumptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockMovementId = table.Column<Guid>(type: "uuid", nullable: false),
                    IngredientLotId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantityConsumed = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: false),
                    UnitPriceAtConsumption = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovementLotConsumptions", x => x.Id);
                    table.CheckConstraint("CK_StockMovementLotConsumption_QuantityConsumed_Positive", "\"QuantityConsumed\" > 0");
                    table.ForeignKey(
                        name: "FK_StockMovementLotConsumptions_IngredientLots_IngredientLotId",
                        column: x => x.IngredientLotId,
                        principalTable: "IngredientLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovementLotConsumptions_StockMovements_StockMovementId",
                        column: x => x.StockMovementId,
                        principalTable: "StockMovements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientLots_IngredientId_PurchasedAt",
                table: "IngredientLots",
                columns: new[] { "IngredientId", "PurchasedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovementLotConsumptions_IngredientLotId",
                table: "StockMovementLotConsumptions",
                column: "IngredientLotId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovementLotConsumptions_StockMovementId",
                table: "StockMovementLotConsumptions",
                column: "StockMovementId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_IngredientId_CreatedAt",
                table: "StockMovements",
                columns: new[] { "IngredientId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_LocationId",
                table: "StockMovements",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_PerformedByUserId",
                table: "StockMovements",
                column: "PerformedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_RelatedOrderId",
                table: "StockMovements",
                column: "RelatedOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockMovementLotConsumptions");

            migrationBuilder.DropTable(
                name: "IngredientLots");

            migrationBuilder.DropTable(
                name: "StockMovements");

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "Ingredients",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Ingredient_UnitPrice_NonNegative",
                table: "Ingredients",
                sql: "\"UnitPrice\" >= 0");
        }
    }
}