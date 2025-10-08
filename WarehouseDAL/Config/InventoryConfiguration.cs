using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseDomain.Entities;

namespace WarehouseDAL.Config;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> b)
    {
        b.ToTable("Inventory");
        b.HasKey(i => i.Id);
        b.Property(i => i.WarehouseId).IsRequired();
        b.Property(i => i.ProductId).IsRequired();
        b.Property(i => i.Quantity).IsRequired();
        b.Property(i => i.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
        b.Property(i => i.CreatedBy).IsRequired();
        b.Property(i => i.UpdatedAt).HasDefaultValueSql("GETDATE()");
        b.Property(i => i.IsDeleted).HasDefaultValue(false);

        b.HasIndex(i => new { i.WarehouseId, i.ProductId }).IsUnique();

        b.HasOne(i => i.Warehouse)
            .WithMany(w => w.Inventories)
            .HasForeignKey(i => i.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(i => i.Product)
            .WithMany(p => p.Inventories)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}