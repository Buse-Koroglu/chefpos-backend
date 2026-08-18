using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Description).HasMaxLength(500);  
        builder.Property(p => p.ImageUrl).HasMaxLength(500);
        builder.Property(p => p.Price).HasPrecision(10, 2);
        builder.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(p => p.ProductLocations).HasField("_locations").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}