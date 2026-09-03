using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefPos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserLocationRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLocationRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLocationRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLocationRoles_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLocationRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLocationRoles_LocationId",
                table: "UserLocationRoles",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLocationRoles_UserId_LocationId_Role",
                table: "UserLocationRoles",
                columns: new[] { "UserId", "LocationId", "Role" },
                unique: true);

            // Backfill: mevcut UserRole x UserLocation kombinasyonlarını çapraz çarpım olarak aktarır.
            // Birden fazla rolü VE birden fazla lokasyonu olan kullanıcılarda, hangi rolün hangi
            // lokasyonda geçerli olduğu bilgisi hiç tutulmadığı için kesin olarak bilinemez; bu yüzden
            // erişim kaybını önlemek amacıyla her rol x her lokasyon kombinasyonu için bir satır oluşturulur.
            // SUPER_ADMIN hariçtir (lokasyona bağlı değildir). Bu kombinasyonlardan doğru olmayanların
            // (örn. birden fazla rol/lokasyonu olan kullanıcılar) migration sonrası yeni arayüzden elle
            // temizlenmesi gerekir.
            migrationBuilder.Sql(@"
                INSERT INTO ""UserLocationRoles"" (""Id"", ""UserId"", ""LocationId"", ""Role"", ""CreatedAt"")
                SELECT gen_random_uuid(), ur.""UserId"", ul.""LocationId"", ur.""Role"", NOW()
                FROM ""UserRoles"" ur
                JOIN ""UserLocations"" ul ON ul.""UserId"" = ur.""UserId""
                WHERE ur.""Role"" <> 'SUPER_ADMIN';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLocationRoles");
        }
    }
}
