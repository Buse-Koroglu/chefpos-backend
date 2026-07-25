using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.PersonalId).IsRequired().HasMaxLength(50);
        builder.HasIndex(u => u.PersonalId).IsUnique();
        builder.Property(u=>u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u=>u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Password).IsRequired();
        builder.Property(u=>u.Role).HasConversion<string>().HasMaxLength(20);
        builder.HasMany(u => u.Orders).WithOne(o => o.Cashier).HasForeignKey(o => o.CashierId).OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(u => u.Orders).HasField("_orders").UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasMany(u => u.Locations).WithOne(ul => ul.User).HasForeignKey(ul => ul.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(u => u.Locations).HasField("_locations").UsePropertyAccessMode(PropertyAccessMode.Field);

    }
}