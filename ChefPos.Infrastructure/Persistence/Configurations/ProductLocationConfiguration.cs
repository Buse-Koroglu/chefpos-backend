using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infastructure.Persistence.Configurations;

public class ProductLocationConfiguration : IEntityTypeConfiguration<ProductLocation>
{
    public void Configure(EntityTypeBuilder<ProductLocation> builder)
    {
        builder.HasKey(pl => pl.Id);
        builder.HasIndex(pl => new { pl.ProductId, pl.LocationId }).IsUnique();
        builder.HasOne(pl => pl.Product).WithMany(p => (ICollection<ProductLocation>)p.ProductLocations).HasForeignKey(pl => pl.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(pl => pl.Location).WithMany().HasForeignKey(pl => pl.LocationId).OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pl => pl.ProductItems).WithOne(pi => pi.ProductLocation).HasForeignKey(pi => pi.ProductLocationId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(pl => pl.ProductItems).HasField("_productItems").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
