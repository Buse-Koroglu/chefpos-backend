using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.TokenHash).IsRequired().HasMaxLength(128);
        builder.HasIndex(rt => rt.TokenHash).IsUnique(); // her bir refresh token'ın token hash değeri unique olmalıdır.
        builder.HasOne(rt => rt.User).WithMany().HasForeignKey(rt => rt.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}