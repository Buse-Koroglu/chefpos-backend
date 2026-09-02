using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infrastructure.Persistence.Configurations;

public class StockMovementLotConsumptionConfiguration : IEntityTypeConfiguration<StockMovementLotConsumption>
{
    public void Configure(EntityTypeBuilder<StockMovementLotConsumption> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.QuantityConsumed).HasPrecision(10, 3);
        builder.Property(c => c.UnitPriceAtConsumption).HasPrecision(10, 2);
        builder.HasOne(c => c.IngredientLot).WithMany().HasForeignKey(c => c.IngredientLotId).OnDelete(DeleteBehavior.Restrict);
        builder.ToTable(c => c.HasCheckConstraint("CK_StockMovementLotConsumption_QuantityConsumed_Positive", "\"QuantityConsumed\" > 0"));
    }
}