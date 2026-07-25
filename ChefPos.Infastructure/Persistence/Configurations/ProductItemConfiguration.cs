using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infastructure.Persistence.Configurations;

public class ProductItemConfiguration : IEntityTypeConfiguration<ProductItem>
{
    public void Configure(EntityTypeBuilder<ProductItem> builder)
    {
      builder.HasKey(pi=>pi.Id);
      builder.Property(pi=>pi.Name).IsRequired().HasMaxLength(100);
      builder.Property(pi=>pi.UnitPrice).HasPrecision(10, 2);
      builder.ToTable(pi => pi.HasCheckConstraint("CK_ProductItem_UnitPrice_NonNegative", "\"UnitPrice\" >= 0"));
      
    }
}