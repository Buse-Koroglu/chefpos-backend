using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StockRequestConfiguration : IEntityTypeConfiguration<StockRequest>
{
    public void Configure(EntityTypeBuilder<StockRequest> builder)
    {
        builder.HasKey(sr => sr.Id);
        builder.Property(sr => sr.RequestedQuantity).HasPrecision(10, 3); // toplam 10 hane virgülden sonra 3
        builder.Property(sr => sr.RejectionReason).HasMaxLength(500);
        builder.Property(sr => sr.ApprovedUnitPrice).HasPrecision(10, 2); // toplam 10 hane virgülden sonra 2

        builder.HasOne(sr => sr.Ingredient).WithMany().HasForeignKey(sr => sr.IngredientId).OnDelete(DeleteBehavior.Restrict); // stock request bir tane ingredient içeriyor ve ingredient ise birden fazla stock request içeriyor

        builder.HasOne(sr => sr.Location).WithMany().HasForeignKey(sr => sr.LocationId).OnDelete(DeleteBehavior.Restrict); // stock request bir tane location'a sahiptir ama bir lokasyonun birden fazla stock request'i olabilir.

        builder.HasOne(sr => sr.RequestedByUser).WithMany().HasForeignKey(sr => sr.RequestedByUserId).OnDelete(DeleteBehavior.Restrict); // stock request'in sadece bir tane requestedUser'ı var ama requestedUser'ın birden fazla stock request'i olabilir.

        builder.HasOne(sr => sr.DecidedByUser).WithMany().HasForeignKey(sr => sr.DecidedByUserId).OnDelete(DeleteBehavior.Restrict); // stock request'in sadece bir tane decidedUser'ı var ama decidedUser'ın birden fazla stock request'i olabilir.

        builder.HasIndex(sr => new { sr.IngredientId, sr.LocationId }).IsUnique().HasFilter("\"Status\" = 0"); // arama işlemini hızlanıdırır ve aynı zamanda bir locationda bir ingredient için birden fazla pending surumunda stock request olmayacağını belirtir

        builder.ToTable(sr => sr.HasCheckConstraint("CK_StockRequest_RequestedQuantity_Positive", "\"RequestedQuantity\" > 0"));
    }
}