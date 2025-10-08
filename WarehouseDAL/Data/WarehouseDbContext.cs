using Microsoft.EntityFrameworkCore;
using WarehouseDomain.Entities;

namespace WarehouseDAL.Data;

public class WarehouseDbContext : DbContext
    {
        public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options)
        {
        }

        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Product> Products { get; set; } 
        public DbSet<Supplier> Suppliers { get; set; } 
        public DbSet<Inventory> Inventory { get; set; } 
        public DbSet<SupplierProduct> SupplierProducts { get; set; } 
        public DbSet<WarehouseDetail> WarehouseDetails { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarehouseDbContext).Assembly);

        }
    }