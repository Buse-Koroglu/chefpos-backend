using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infastructure.Persistence.Configurations;

public class CategoryLocationConfiguration : IEntityTypeConfiguration<CategoryLocation>
{
    public void Configure(EntityTypeBuilder<CategoryLocation> builder)
    {
        builder.HasKey(cl => cl.Id);
        builder.HasIndex(cl => new { cl.CategoryId, cl.LocationId }).IsUnique();
        builder.HasOne(cl => cl.Category).WithMany(c => (ICollection<CategoryLocation>)c.CategoryLocations).HasForeignKey(cl => cl.CategoryId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(cl => cl.Location).WithMany().HasForeignKey(cl => cl.LocationId).OnDelete(DeleteBehavior.Cascade);
    }
}