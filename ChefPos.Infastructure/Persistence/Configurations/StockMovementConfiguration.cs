using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infastructure.Persistence.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Quantity).HasPrecision(10, 3);
        builder.Property(m => m.Note).HasMaxLength(500);

        builder.HasOne(m => m.Ingredient)
            .WithMany()
            .HasForeignKey(m => m.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Location)
            .WithMany()
            .HasForeignKey(m => m.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.PerformedByUser)
            .WithMany()
            .HasForeignKey(m => m.PerformedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.LotConsumptions)
            .WithOne(c => c.StockMovement)
            .HasForeignKey(c => c.StockMovementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(m => m.WeightedUnitPrice);

        builder.HasIndex(m => new { m.IngredientId, m.CreatedAt });
        builder.HasIndex(m => m.RelatedOrderId);

        builder.ToTable(m => m.HasCheckConstraint("CK_StockMovement_Quantity_Positive", "\"Quantity\" > 0"));
    }
}