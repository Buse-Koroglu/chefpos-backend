using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefPos.Infastructure.Persistence.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.TableNumber).IsRequired();

        builder.HasOne(t => t.Location).WithMany().HasForeignKey(t => t.LocationId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.LocationId, t.TableNumber }).IsUnique();
    }
}
