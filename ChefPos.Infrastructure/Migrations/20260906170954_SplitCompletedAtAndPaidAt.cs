using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChefPos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitCompletedAtAndPaidAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            // Mevcut CompletedAt değerleri aslında ödeme zamanıydı (eski MarkAsPaid() bu alanı dolduruyordu).
            migrationBuilder.Sql(@"UPDATE ""Orders"" SET ""PaidAt"" = ""CompletedAt"" WHERE ""CompletedAt"" IS NOT NULL;");

            // Tamamlanmamış siparişlerde (ör. mutfak henüz tamamlamadan ödenen kiosk siparişleri)
            // CompletedAt yanlışlıkla dolu kalmasın; gerçek tamamlanma zamanı yalnızca COMPLETED durumundakiler için biliniyor,
            // o da en yakın tahminle eski (ödeme) zamanı.
            migrationBuilder.Sql(@"UPDATE ""Orders"" SET ""CompletedAt"" = NULL WHERE ""OrderStatus"" <> 'COMPLETED';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Orders");
        }
    }
}
