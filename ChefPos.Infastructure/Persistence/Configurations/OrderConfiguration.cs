using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o=>o.Id);
        builder.Property(o=>o.OrderNumber).ValueGeneratedOnAdd();
        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.Property(o => o.OrderStatus).HasConversion<string>().HasMaxLength(20);
        builder.Property(o => o.PaymentStatus).HasConversion<string>().HasMaxLength(20);
        builder.Property(o => o.OrderType).HasConversion<string>().HasMaxLength(20);
        builder.Property(o=>o.CustomerName).IsRequired().HasMaxLength(100);
        builder.Property(o => o.TotalPrice).HasPrecision(10, 2);
        builder.HasOne(o=>o.Location).WithMany(l=>l.Orders).HasForeignKey(o=>o.LocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(o=>o.Items).WithOne(oi=>oi.Order).HasForeignKey(oi=>oi.OrderId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(o=>o.Items).HasField("_items").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
    
}