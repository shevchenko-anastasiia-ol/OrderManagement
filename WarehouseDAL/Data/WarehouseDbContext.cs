using Microsoft.EntityFrameworkCore;
using WarehouseDomain.Entities;

namespace WarehouseDAL.Data;

public class WarehouseDbContext : DbContext
    {
        public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options)
        {
        }

        public DbSet<Warehouse> Warehouses { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<Inventory> Inventory { get; set; } = null!;
        public DbSet<SupplierProduct> SupplierProducts { get; set; } = null!;
        public DbSet<WarehouseDetail> WarehouseDetails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Warehouse configuration
            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Capacity).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.CreatedBy).IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                
                entity.HasMany(e => e.Inventories)
                    .WithOne(i => i.Warehouse)
                    .HasForeignKey(i => i.WarehouseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Details)
                    .WithOne(d => d.Warehouse)
                    .HasForeignKey(d => d.WarehouseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.SKU).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.CreatedBy).IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.HasIndex(e => e.SKU).IsUnique();

                entity.HasMany(e => e.Inventories)
                    .WithOne(i => i.Product)
                    .HasForeignKey(i => i.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.SupplierProducts)
                    .WithOne(sp => sp.Product)
                    .HasForeignKey(sp => sp.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Supplier configuration
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ContactInfo).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.CreatedBy).IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.HasMany(e => e.SupplierProducts)
                    .WithOne(sp => sp.Supplier)
                    .HasForeignKey(sp => sp.SupplierId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Inventory configuration
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.WarehouseId).IsRequired();
                entity.Property(e => e.ProductId).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.CreatedBy).IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.HasIndex(e => new { e.WarehouseId, e.ProductId }).IsUnique();
            });

            // SupplierProducts configuration
            modelBuilder.Entity<SupplierProduct>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SupplierId).IsRequired();
                entity.Property(e => e.ProductId).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.CreatedBy).IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.HasIndex(e => new { e.SupplierId, e.ProductId }).IsUnique();
            });

            // WarehouseDetails configuration
            modelBuilder.Entity<WarehouseDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.WarehouseId).IsRequired();
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.Manager).HasMaxLength(200);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.CreatedBy).IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.HasIndex(e => e.WarehouseId).IsUnique();
            });
        }
    }