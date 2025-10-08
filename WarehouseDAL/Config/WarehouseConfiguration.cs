using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseDomain.Entities;

namespace WarehouseDAL.Config;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> b)
    {
        b.ToTable("Warehouses");
        b.HasKey(w => w.Id);
        b.Property(w => w.Name).IsRequired().HasMaxLength(200);
        b.Property(w => w.Capacity).IsRequired();
        b.Property(w => w.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
        b.Property(w => w.CreatedBy).IsRequired();
        b.Property(w => w.UpdatedAt).HasDefaultValueSql("GETDATE()");
        b.Property(w => w.IsDeleted).HasDefaultValue(false);

        b.HasMany(w => w.Inventories)
            .WithOne(i => i.Warehouse)
            .HasForeignKey(i => i.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(w => w.Details)
            .WithOne(d => d.Warehouse)
            .HasForeignKey(d => d.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}