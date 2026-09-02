using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefPos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPackageToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPackage",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPackage",
                table: "Orders");
        }
    }
}
