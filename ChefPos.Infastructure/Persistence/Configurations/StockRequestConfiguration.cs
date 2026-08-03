using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StockRequestConfiguration : IEntityTypeConfiguration<StockRequest>
{
    public void Configure(EntityTypeBuilder<StockRequest> builder)
    {
        builder.HasKey(sr => sr.Id);
        builder.Property(sr => sr.RequestedQuantity).HasPrecision(10, 3);
        builder.Property(sr => sr.RejectionReason).HasMaxLength(500);

        builder.HasOne(sr => sr.Ingredient)
            .WithMany()
            .HasForeignKey(sr => sr.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sr => sr.Location)
            .WithMany()
            .HasForeignKey(sr => sr.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sr => sr.RequestedByUser)
            .WithMany()
            .HasForeignKey(sr => sr.RequestedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sr => sr.DecidedByUser)
            .WithMany()
            .HasForeignKey(sr => sr.DecidedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(sr => new { sr.IngredientId, sr.LocationId })
            .IsUnique()
            .HasFilter("\"Status\" = 0");

        builder.ToTable(sr => sr.HasCheckConstraint("CK_StockRequest_RequestedQuantity_Positive", "\"RequestedQuantity\" > 0"));
    }
}