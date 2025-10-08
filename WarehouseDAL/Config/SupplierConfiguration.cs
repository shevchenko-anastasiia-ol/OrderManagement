using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseDomain.Entities;

namespace WarehouseDAL.Config;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> b)
    {
        b.ToTable("Suppliers");
        b.HasKey(s => s.Id);
        b.Property(s => s.Name).IsRequired().HasMaxLength(200);
        b.Property(s => s.Country).IsRequired().HasMaxLength(100);
        b.Property(s => s.ContactInfo).HasMaxLength(500);
        b.Property(s => s.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
        b.Property(s => s.CreatedBy).IsRequired();
        b.Property(s => s.UpdatedAt).HasDefaultValueSql("GETDATE()");
        b.Property(s => s.IsDeleted).HasDefaultValue(false);

        b.HasMany(s => s.SupplierProducts)
            .WithOne(sp => sp.Supplier)
            .HasForeignKey(sp => sp.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}