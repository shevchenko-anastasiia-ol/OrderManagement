using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseDomain.Entities;

namespace WarehouseDAL.Config;

public class WarehouseDetailConfiguration : IEntityTypeConfiguration<WarehouseDetail>
{
    public void Configure(EntityTypeBuilder<WarehouseDetail> b)
    {
        b.ToTable("WarehouseDetails");
        b.HasKey(wd => wd.Id);
        b.Property(wd => wd.WarehouseId).IsRequired();
        b.Property(wd => wd.Address).HasMaxLength(500);
        b.Property(wd => wd.Manager).HasMaxLength(200);
        b.Property(wd => wd.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
        b.Property(wd => wd.CreatedBy).IsRequired();
        b.Property(wd => wd.UpdatedAt).HasDefaultValueSql("GETDATE()");
        b.Property(wd => wd.IsDeleted).HasDefaultValue(false);

        b.HasIndex(wd => wd.WarehouseId).IsUnique();

        b.HasOne(wd => wd.Warehouse)
            .WithMany(w => w.Details)
            .HasForeignKey(wd => wd.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}