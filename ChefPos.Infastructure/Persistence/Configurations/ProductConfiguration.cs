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
        builder.HasOne(p=>p.Location).WithMany(l=>l.Products).HasForeignKey(p => p.LocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(p=>p.ProductItems).WithOne(pi=>pi.Product).HasForeignKey(pi=>pi.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(p => p.ProductItems).HasField("_productItems").UsePropertyAccessMode(PropertyAccessMode.Field);
        
    }
}