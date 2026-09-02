using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infrastructure.Persistence.Configurations;

public class ProductItemConfiguration : IEntityTypeConfiguration<ProductItem>
{
    public void Configure(EntityTypeBuilder<ProductItem> builder)
    {
        builder.HasKey(pi => pi.Id);
        builder.Property(pi => pi.QuantityPerServing).HasPrecision(10, 3);

        builder.HasOne(pi => pi.Ingredient)
            .WithMany(i => i.ProductItems)
            .HasForeignKey(pi => pi.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(pi => pi.HasCheckConstraint("CK_ProductItem_QuantityPerServing_Positive", "\"QuantityPerServing\" > 0"));
    }
}