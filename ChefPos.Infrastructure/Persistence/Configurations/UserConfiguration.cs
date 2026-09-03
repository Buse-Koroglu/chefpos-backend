using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.PersonalId).IsRequired().HasMaxLength(50);
        builder.HasIndex(u => u.PersonalId).IsUnique();  // unique personal id
        builder.Property(u=>u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u=>u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Password).IsRequired();
        builder.HasMany(u => u.Orders).WithOne(o => o.CreatedByUser).HasForeignKey(o => o.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(u => u.Orders).HasField("_orders").UsePropertyAccessMode(PropertyAccessMode.Field);  // orders property'sini kullanma, verileri doğrudan _orders field'ına yaz/oku.
        builder.HasMany(u => u.Locations).WithOne(ul => ul.User).HasForeignKey(ul => ul.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(u => u.Locations).HasField("_locations").UsePropertyAccessMode(PropertyAccessMode.Field); // orders property'sini kullanma, verileri doğrudan _orders field'ına yaz/oku.
        builder.HasMany(u => u.UserRoles).WithOne(r => r.User).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(u => u.UserRoles).HasField("_roles").UsePropertyAccessMode(PropertyAccessMode.Field); // orders property'sini kullanma, verileri doğrudan _orders field'ına yaz/oku.
        builder.Ignore(u => u.Roles); // veritabanına işlenmemesi için
        builder.HasMany(u => u.LocationRoles).WithOne(lr => lr.User).HasForeignKey(lr => lr.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(u => u.LocationRoles).HasField("_locationRoles").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}