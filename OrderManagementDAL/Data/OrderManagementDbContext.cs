using MarketplaceDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace OrderManagementDAL.Data;

public class OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Shipment> Shipments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Email).IsUnique();
        modelBuilder.Entity<Customer>()
            .Property(c => c.FirstName)
            .HasMaxLength(50);
        modelBuilder.Entity<Customer>()
            .Property(c => c.LastName)
            .HasMaxLength(50);
        modelBuilder.Entity<Customer>()
            .Property(c => c.Email)
            .HasMaxLength(100);
        modelBuilder.Entity<Customer>()
            .Property(c => c.Phone)
            .HasMaxLength(20);
        modelBuilder.Entity<Customer>()
            .Property(c => c.CreatedBy)
            .HasMaxLength(50);
        modelBuilder.Entity<Customer>()
            .Property(c => c.UpdatedBy)
            .HasMaxLength(50);
        modelBuilder.Entity<Customer>()
            .Property(c => c.CreatedAt) 
            .HasDefaultValueSql("SYSDATETIME()");
        modelBuilder.Entity<Customer>()
            .Property(c => c.UpdatedAt)
            .IsRequired(false);                 
        modelBuilder.Entity<Customer>()
            .Property(c => c.IsDeleted)
            .HasDefaultValue(false);             
        modelBuilder.Entity<Customer>()
            .Property(c => c.RowVer)
            .IsRowVersion();
        


        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.OrderItemId);

            entity.Property(oi => oi.Quantity)
                .IsRequired();

            entity.Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(10,2)");

            entity.Property(oi => oi.CreatedAt)
                .HasDefaultValueSql("SYSDATETIME()");

            entity.Property(oi => oi.CreatedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("suser_sname()");

            entity.Property(oi => oi.UpdatedAt)
                .IsRequired(false);

            entity.Property(oi => oi.UpdatedBy)
                .HasMaxLength(50);

            entity.Property(oi => oi.IsDeleted)
                .HasDefaultValue(false);

            entity.Property(oi => oi.RowVer)
                .IsRowVersion();
            
            entity.HasOne<Order>()
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            entity.HasOne<Product>()
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.OrderId);
            
            entity.HasOne<Customer>()
                .WithMany(c => c.Orders)   
                .HasForeignKey(o => o.CustomerId);

            // OrderDate — datetime2 + дефолт sysdatetime()
            entity.Property(o => o.OrderDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");
            
            entity.Property(o => o.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Pending");
            
            entity.Property(o => o.TotalAmount)
                .HasColumnType("decimal(10,2)");
            
            entity.Property(o => o.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");
            
            entity.Property(o => o.CreatedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("SUSER_SNAME()");
            
            entity.Property(o => o.UpdatedAt)
                .HasColumnType("datetime2")
                .IsRequired(false);
            
            entity.Property(o => o.UpdatedBy)
                .HasMaxLength(50);
            
            entity.Property(o => o.IsDeleted)
                .HasDefaultValue(false);
            
            entity.Property(o => o.RowVer)
                .IsRowVersion();
        });
        
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.PaymentId);
    
            entity.HasOne<Order>()
                .WithMany(o => o.Payments)   
                .HasForeignKey(p => p.OrderId);
            
            entity.Property(p => p.PaymentDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");
    
            entity.Property(p => p.Amount)
                .HasColumnType("decimal(10,2)");
    
            entity.Property(p => p.PaymentMethod)
                .HasMaxLength(30);
    
            entity.Property(p => p.PaymentStatus)
                .HasMaxLength(30)
                .HasDefaultValue("Pending");
    
            entity.Property(p => p.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");
    
            entity.Property(p => p.CreatedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("SUSER_SNAME()");
    
            entity.Property(p => p.UpdatedAt)
                .HasColumnType("datetime2")
                .IsRequired(false);
    
            entity.Property(p => p.UpdatedBy)
                .HasMaxLength(50);
    
            entity.Property(p => p.IsDeleted)
                .HasDefaultValue(false);
    
            entity.Property(p => p.RowVer)
                .IsRowVersion();
        });
        
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.ProductId);
    
            entity.Property(p => p.ProductName)
                .HasMaxLength(100);
    
            entity.Property(p => p.Description)
                .HasMaxLength(255);
    
            entity.Property(p => p.Price)
                .HasColumnType("decimal(10,2)");
    
            entity.Property(p => p.StockQuantity)
                .HasDefaultValue(0);
    
            entity.Property(p => p.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");
    
            entity.Property(p => p.CreatedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("SUSER_SNAME()");
    
            entity.Property(p => p.UpdatedAt)
                .HasColumnType("datetime2")
                .IsRequired(false);
    
            entity.Property(p => p.UpdatedBy)
                .HasMaxLength(50);
    
            entity.Property(p => p.IsDeleted)
                .HasDefaultValue(false);
    
            entity.Property(p => p.RowVer)
                .IsRowVersion();
        });
        
        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(s => s.ShipmentId);
    
            entity.HasOne<Order>()
                .WithMany(o => o.Shipments)   
                .HasForeignKey(s => s.OrderId);

            entity.Property(s => s.ShipmentDate)
                .HasColumnType("datetime2");
    
            entity.Property(s => s.TrackingNumber)
                .HasMaxLength(50);
    
            entity.Property(s => s.Carrier)
                .HasMaxLength(50);
    
            entity.Property(s => s.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Processing");
    
            entity.Property(s => s.AddressLine1)
                .HasMaxLength(150);
    
            entity.Property(s => s.AddressLine2)
                .HasMaxLength(150);
    
            entity.Property(s => s.City)
                .HasMaxLength(50);
    
            entity.Property(s => s.Region)
                .HasMaxLength(50);
    
            entity.Property(s => s.PostalCode)
                .HasMaxLength(20);
    
            entity.Property(s => s.Country)
                .HasMaxLength(50)
                .HasDefaultValue("Ukraine");
    
            entity.Property(s => s.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");
    
            entity.Property(s => s.CreatedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("SUSER_SNAME()");
    
            entity.Property(s => s.UpdatedAt)
                .HasColumnType("datetime2")
                .IsRequired(false);
    
            entity.Property(s => s.UpdatedBy)
                .HasMaxLength(50);
    
            entity.Property(s => s.IsDeleted)
                .HasDefaultValue(false);
    
            entity.Property(s => s.RowVer)
                .IsRowVersion();
        });




    }
}





