using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infastructure.Persistence.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Description).HasMaxLength(500);
        builder.HasOne(m => m.Location).WithMany().HasForeignKey(m => m.LocationId).OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(m => m.MenuProducts).HasField("_menuProducts").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}