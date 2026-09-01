using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefPos.Infastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertProductLocationToMultiLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductItems_Products_ProductId",
                table: "ProductItems");

            migrationBuilder.CreateTable(
                name: "ProductLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductLocations_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductLocations_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocations_LocationId",
                table: "ProductLocations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLocations_ProductId_LocationId",
                table: "ProductLocations",
                columns: new[] { "ProductId", "LocationId" },
                unique: true);

            // Backfill: one ProductLocation per existing product, carrying over its old single LocationId.
            migrationBuilder.Sql(@"
                INSERT INTO ""ProductLocations"" (""Id"", ""ProductId"", ""LocationId"", ""CreatedAt"")
                SELECT gen_random_uuid(), ""Id"", ""LocationId"", NOW()
                FROM ""Products"";
            ");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "ProductItems",
                newName: "ProductLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductItems_ProductId",
                table: "ProductItems",
                newName: "IX_ProductItems_ProductLocationId");

            // Retarget existing recipe lines from the old Product.Id to the new ProductLocation.Id
            // created above for that same product (1:1 at migration time, since each product had one location).
            migrationBuilder.Sql(@"
                UPDATE ""ProductItems"" pi
                SET ""ProductLocationId"" = pl.""Id""
                FROM ""ProductLocations"" pl
                WHERE pl.""ProductId"" = pi.""ProductLocationId"";
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductItems_ProductLocations_ProductLocationId",
                table: "ProductItems",
                column: "ProductLocationId",
                principalTable: "ProductLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Locations_LocationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_LocationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductItems_ProductLocations_ProductLocationId",
                table: "ProductItems");

            migrationBuilder.DropTable(
                name: "ProductLocations");

            migrationBuilder.RenameColumn(
                name: "ProductLocationId",
                table: "ProductItems",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductItems_ProductLocationId",
                table: "ProductItems",
                newName: "IX_ProductItems_ProductId");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "Products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Products_LocationId",
                table: "Products",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductItems_Products_ProductId",
                table: "ProductItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Locations_LocationId",
                table: "Products",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
