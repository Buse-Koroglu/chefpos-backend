using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infrastructure.Persistence.Configurations;

public class UserLocationRoleConfiguration : IEntityTypeConfiguration<UserLocationRole>
{
    public void Configure(EntityTypeBuilder<UserLocationRole> builder)
    {
        builder.HasKey(lr => lr.Id);
        builder.HasIndex(lr => new { lr.UserId, lr.LocationId, lr.Role }).IsUnique();
        builder.Property(lr => lr.Role).HasConversion<string>().HasMaxLength(50);
        builder.HasOne(lr => lr.Location).WithMany().HasForeignKey(lr => lr.LocationId).OnDelete(DeleteBehavior.Cascade);
    }
}
