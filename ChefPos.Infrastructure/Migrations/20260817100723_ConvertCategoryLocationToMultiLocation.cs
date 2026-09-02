using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefPos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertCategoryLocationToMultiLocation : Migration
    {
        /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateTable(
        name: "CategoryLocations",
        columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
            LocationId = table.Column<Guid>(type: "uuid", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_CategoryLocations", x => x.Id);
            table.ForeignKey(
                name: "FK_CategoryLocations_Categories_CategoryId",
                column: x => x.CategoryId,
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                name: "FK_CategoryLocations_Locations_LocationId",
                column: x => x.LocationId,
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        });

    migrationBuilder.CreateIndex(
        name: "IX_CategoryLocations_CategoryId_LocationId",
        table: "CategoryLocations",
        columns: new[] { "CategoryId", "LocationId" },
        unique: true);

    migrationBuilder.CreateIndex(
        name: "IX_CategoryLocations_LocationId",
        table: "CategoryLocations",
        column: "LocationId");

    migrationBuilder.Sql(@"
        INSERT INTO ""CategoryLocations"" (""Id"", ""CategoryId"", ""LocationId"", ""CreatedAt"")
        SELECT gen_random_uuid(), ""Id"", ""LocationId"", NOW()
        FROM ""Categories"";
    ");

    migrationBuilder.DropForeignKey(
        name: "FK_Categories_Locations_LocationId",
        table: "Categories");

    migrationBuilder.DropIndex(
        name: "IX_Categories_LocationId",
        table: "Categories");

    migrationBuilder.DropColumn(
        name: "LocationId",
        table: "Categories");
}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryLocations");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "Categories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Categories_LocationId",
                table: "Categories",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Locations_LocationId",
                table: "Categories",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
