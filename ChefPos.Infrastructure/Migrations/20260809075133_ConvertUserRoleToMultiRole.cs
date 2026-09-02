using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefPos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertUserRoleToMultiRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Yeni tablo oluşturulur
            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 2) Index
            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId_Role",
                table: "UserRoles",
                columns: new[] { "UserId", "Role" },
                unique: true);

            // 3) Veri taşıma — tablo var, eski Role kolonu hâlâ var
            migrationBuilder.Sql(@"
                INSERT INTO ""UserRoles"" (""Id"", ""UserId"", ""Role"", ""CreatedAt"")
                SELECT gen_random_uuid(), ""Id"", ""Role"", NOW()
                FROM ""Users"";
            ");

            // 4) Artık eski kolon güvenle silinebilir
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE ""Users"" u
                SET ""Role"" = sub.""Role""
                FROM (
                    SELECT DISTINCT ON (""UserId"") ""UserId"", ""Role""
                    FROM ""UserRoles""
                    ORDER BY ""UserId"", ""Role""
                ) sub
                WHERE u.""Id"" = sub.""UserId"";
            ");

            migrationBuilder.DropTable(
                name: "UserRoles");
        }
    }
}