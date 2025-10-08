using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseDomain.Entities;

namespace WarehouseDAL.Config;

public class SupplierProductConfiguration : IEntityTypeConfiguration<SupplierProduct>
{
    public void Configure(EntityTypeBuilder<SupplierProduct> b)
    {
        b.ToTable("SupplierProducts");
        b.HasKey(sp => sp.Id);
        b.Property(sp => sp.SupplierId).IsRequired();
        b.Property(sp => sp.ProductId).IsRequired();
        b.Property(sp => sp.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
        b.Property(sp => sp.CreatedBy).IsRequired();
        b.Property(sp => sp.UpdatedAt).HasDefaultValueSql("GETDATE()");
        b.Property(sp => sp.IsDeleted).HasDefaultValue(false);

        b.HasIndex(sp => new { sp.SupplierId, sp.ProductId }).IsUnique();

        b.HasOne(sp => sp.Supplier)
            .WithMany(s => s.SupplierProducts)
            .HasForeignKey(sp => sp.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(sp => sp.Product)
            .WithMany(p => p.SupplierProducts)
            .HasForeignKey(sp => sp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}