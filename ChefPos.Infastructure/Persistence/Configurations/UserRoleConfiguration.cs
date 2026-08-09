using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(ur => ur.Id);
        builder.HasIndex(ur => new { ur.UserId, ur.Role }).IsUnique();
        builder.Property(ur => ur.Role).HasConversion<string>().HasMaxLength(50);
    }
}
