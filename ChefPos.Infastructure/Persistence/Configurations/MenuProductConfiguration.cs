using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infastructure.Persistence.Configurations;

public class MenuProductConfiguration : IEntityTypeConfiguration<MenuProduct>
{
    public void Configure(EntityTypeBuilder<MenuProduct> builder)
    {
        builder.HasKey(mp => mp.Id);
        builder.HasIndex(mp => new { mp.MenuId, mp.ProductId }).IsUnique();
        builder.HasOne(mp => mp.Menu).WithMany(m => (ICollection<MenuProduct>)m.MenuProducts).HasForeignKey(mp => mp.MenuId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(mp => mp.Product).WithMany().HasForeignKey(mp => mp.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}