using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.Id).ValueGeneratedNever();
        builder.Property(oi=>oi.Name).IsRequired().HasMaxLength(100);
        builder.Property(oi=>oi.Price).HasPrecision(10, 2);
        builder.ToTable(oi => oi.HasCheckConstraint("CK_OrderItem_Quantity_Positive", "\"Quantity\" > 0"));

    }
}