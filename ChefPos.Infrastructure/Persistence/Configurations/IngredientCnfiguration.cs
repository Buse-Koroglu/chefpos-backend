using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infastructure.Persistence.Configurations;

 
public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Name).IsRequired().HasMaxLength(100);
        builder.Property(i => i.CurrentStock).HasPrecision(10, 3);
        builder.Property(i => i.MinStockThreshold).HasPrecision(10, 3);
        builder.HasOne(i => i.Location).WithMany(l => l.Ingredients).HasForeignKey(i => i.LocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(i => i.Lots).WithOne(l => l.Ingredient).HasForeignKey(l => l.IngredientId).OnDelete(DeleteBehavior.Restrict);
        builder.Ignore(i => i.LatestUnitPrice);
        builder.Ignore(i => i.WeightedAverageUnitPrice); // tabloda kaydedilmez.
        builder.Ignore(i => i.IsBellowThreshold);
        builder.HasIndex(i => new { i.LocationId, i.Name }).IsUnique();  // aynı lokasyonsa aynı isimli ham madde üretilemez
        builder.ToTable(i => i.HasCheckConstraint("CK_Ingredient_CurrentStock_NonNegative", "\"CurrentStock\" >= 0"));
        builder.ToTable(i => i.HasCheckConstraint("CK_Ingredient_MinStockThreshold_NonNegative", "\"MinStockThreshold\" >= 0"));
    }
}