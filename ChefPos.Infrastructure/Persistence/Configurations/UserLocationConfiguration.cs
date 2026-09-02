using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infrastructure.Persistence.Configurations;

public class UserLocationConfiguration : IEntityTypeConfiguration<UserLocation>
{
    public void Configure(EntityTypeBuilder<UserLocation> builder)
    {
        builder.HasKey(ul=>ul.Id);
        builder.HasIndex(ul=> new { ul.UserId,ul.LocationId }).IsUnique();
        builder.HasOne(ul => ul.Location).WithMany(l => l.AuthorizedUsers).HasForeignKey(ul => ul.LocationId).OnDelete(DeleteBehavior.Cascade);
    }
}