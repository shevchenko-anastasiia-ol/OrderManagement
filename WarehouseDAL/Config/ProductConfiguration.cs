using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseDomain.Entities;

namespace WarehouseDAL.Config;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("Products");
        b.HasKey(p => p.Id);
        b.Property(p => p.Name).IsRequired().HasMaxLength(200);
        b.Property(p => p.SKU).IsRequired().HasMaxLength(100);
        b.HasIndex(p => p.SKU).IsUnique();
        b.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
        b.Property(p => p.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
        b.Property(p => p.CreatedBy).IsRequired();
        b.Property(p => p.UpdatedAt).HasDefaultValueSql("GETDATE()");
        b.Property(p => p.IsDeleted).HasDefaultValue(false);

        b.HasMany(p => p.Inventories)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(p => p.SupplierProducts)
            .WithOne(sp => sp.Product)
            .HasForeignKey(sp => sp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}