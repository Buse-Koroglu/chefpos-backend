using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infrastructure.Persistence.Configurations;

public class IngredientLotConfiguration : IEntityTypeConfiguration<IngredientLot>
{
    public void Configure(EntityTypeBuilder<IngredientLot> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.InitialQuantity).HasPrecision(10, 3);
        builder.Property(l => l.RemainingQuantity).HasPrecision(10, 3);
        builder.Property(l => l.UnitPrice).HasPrecision(10, 2);
        builder.HasIndex(l => new { l.IngredientId, l.PurchasedAt });
        builder.ToTable(l => l.HasCheckConstraint("CK_IngredientLot_InitialQuantity_Positive", "\"InitialQuantity\" > 0"));
        builder.ToTable(l => l.HasCheckConstraint("CK_IngredientLot_RemainingQuantity_NonNegative", "\"RemainingQuantity\" >= 0"));
        builder.ToTable(l => l.HasCheckConstraint("CK_IngredientLot_UnitPrice_NonNegative", "\"UnitPrice\" >= 0"));
    }
}