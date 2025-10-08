using Bogus;
using Microsoft.EntityFrameworkCore;
using WarehouseDomain.Entities;

namespace WarehouseDAL.Data;

public class WarehouseSeedDb
{
    private readonly WarehouseDbContext _context;
        private const int WAREHOUSES_COUNT = 10;
        private const int PRODUCTS_COUNT = 50;
        private const int SUPPLIERS_COUNT = 20;
        private const int INVENTORY_COUNT = 100;

        public WarehouseSeedDb(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.Warehouses.AnyAsync())
            {
                Console.WriteLine("Database already seeded. Skipping...");
                return;
            }

            Console.WriteLine("Starting database seeding...");

            await SeedWarehousesAsync();
            await SeedProductsAsync();
            await SeedSuppliersAsync();
            await SeedInventoryAsync();
            await SeedSupplierProductsAsync();
            await SeedWarehouseDetailsAsync();

            Console.WriteLine("Database seeding completed successfully!");
        }

        private async Task SeedWarehousesAsync()
        {
            Console.WriteLine("Seeding warehouses...");

            var warehouseFaker = new Faker<Warehouse>()
                .RuleFor(w => w.Name, f => f.Company.CompanyName() + " Warehouse")
                .RuleFor(w => w.Capacity, f => f.Random.Int(1000, 10000))
                .RuleFor(w => w.CreatedAt, f => f.Date.Past(2))
                .RuleFor(w => w.CreatedBy, f => f.Random.Int(1, 10))
                .RuleFor(w => w.UpdatedAt, f => f.Date.Recent(30))
                .RuleFor(w => w.UpdatedBy, f => f.Random.Int(1, 10))
                .RuleFor(w => w.IsDeleted, f => false)
                .RuleFor(w => w.RowVersion, f => f.Date.Recent(10));

            var warehouses = warehouseFaker.Generate(WAREHOUSES_COUNT);
            await _context.Warehouses.AddRangeAsync(warehouses);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Seeded {WAREHOUSES_COUNT} warehouses");
        }

        private async Task SeedProductsAsync()
        {
            Console.WriteLine("Seeding products...");

            var productCategories = new[] { "Electronics", "Furniture", "Clothing", "Food", "Books", "Toys", "Sports", "Tools" };

            var productFaker = new Faker<Product>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.SKU, f => f.Commerce.Ean13())
                .RuleFor(p => p.Price, f => f.Finance.Amount(10, 5000, 2))
                .RuleFor(p => p.CreatedAt, f => f.Date.Past(2))
                .RuleFor(p => p.CreatedBy, f => f.Random.Int(1, 10))
                .RuleFor(p => p.UpdatedAt, f => f.Date.Recent(30))
                .RuleFor(p => p.UpdatedBy, f => f.Random.Int(1, 10))
                .RuleFor(p => p.IsDeleted, f => false)
                .RuleFor(p => p.RowVersion, f => f.Date.Recent(10));

            var products = productFaker.Generate(PRODUCTS_COUNT);
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Seeded {PRODUCTS_COUNT} products");
        }

        private async Task SeedSuppliersAsync()
        {
            Console.WriteLine("Seeding suppliers...");

            var supplierFaker = new Faker<Supplier>()
                .RuleFor(s => s.Name, f => f.Company.CompanyName())
                .RuleFor(s => s.Country, f => f.Address.Country())
                .RuleFor(s => s.ContactInfo, f => f.Phone.PhoneNumber() + " | " + f.Internet.Email())
                .RuleFor(s => s.CreatedAt, f => f.Date.Past(2))
                .RuleFor(s => s.CreatedBy, f => f.Random.Int(1, 10))
                .RuleFor(s => s.UpdatedAt, f => f.Date.Recent(30))
                .RuleFor(s => s.UpdatedBy, f => f.Random.Int(1, 10))
                .RuleFor(s => s.IsDeleted, f => false)
                .RuleFor(s => s.RowVersion, f => f.Date.Recent(10));

            var suppliers = supplierFaker.Generate(SUPPLIERS_COUNT);
            await _context.Suppliers.AddRangeAsync(suppliers);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Seeded {SUPPLIERS_COUNT} suppliers");
        }

        private async Task SeedInventoryAsync()
        {
            Console.WriteLine("Seeding inventory...");

            var warehouses = await _context.Warehouses.ToListAsync();
            var products = await _context.Products.ToListAsync();

            var inventoryFaker = new Faker<Inventory>()
                .RuleFor(i => i.WarehouseId, f => f.PickRandom(warehouses).Id)
                .RuleFor(i => i.ProductId, f => f.PickRandom(products).Id)
                .RuleFor(i => i.Quantity, f => f.Random.Int(0, 1000))
                .RuleFor(i => i.CreatedAt, f => f.Date.Past(1))
                .RuleFor(i => i.CreatedBy, f => f.Random.Int(1, 10))
                .RuleFor(i => i.UpdatedAt, f => f.Date.Recent(30))
                .RuleFor(i => i.UpdatedBy, f => f.Random.Int(1, 10))
                .RuleFor(i => i.IsDeleted, f => false)
                .RuleFor(i => i.RowVersion, f => f.Date.Recent(10));

            var inventories = new List<Inventory>();
            var uniquePairs = new HashSet<(int, int)>();

            // Generate unique warehouse-product combinations
            while (inventories.Count < INVENTORY_COUNT)
            {
                var inventory = inventoryFaker.Generate();
                var pair = (inventory.WarehouseId, inventory.ProductId);

                if (uniquePairs.Add(pair))
                {
                    inventories.Add(inventory);
                }
            }

            await _context.Inventory.AddRangeAsync(inventories);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Seeded {INVENTORY_COUNT} inventory records");
        }

        private async Task SeedSupplierProductsAsync()
        {
            Console.WriteLine("Seeding supplier-product relationships...");

            var suppliers = await _context.Suppliers.ToListAsync();
            var products = await _context.Products.ToListAsync();

            var supplierProductFaker = new Faker<SupplierProduct>()
                .RuleFor(sp => sp.SupplierId, f => f.PickRandom(suppliers).Id)
                .RuleFor(sp => sp.ProductId, f => f.PickRandom(products).Id)
                .RuleFor(sp => sp.CreatedAt, f => f.Date.Past(1))
                .RuleFor(sp => sp.CreatedBy, f => f.Random.Int(1, 10))
                .RuleFor(sp => sp.UpdatedAt, f => f.Date.Recent(30))
                .RuleFor(sp => sp.UpdatedBy, f => f.Random.Int(1, 10))
                .RuleFor(sp => sp.IsDeleted, f => false)
                .RuleFor(sp => sp.RowVersion, f => f.Date.Recent(10));

            var supplierProducts = new List<SupplierProduct>();
            var uniquePairs = new HashSet<(int, int)>();

            // Generate unique supplier-product combinations (2-5 products per supplier)
            foreach (var supplier in suppliers)
            {
                var productsCount = new Random().Next(2, 6);
                var availableProducts = products.OrderBy(x => Guid.NewGuid()).Take(productsCount);

                foreach (var product in availableProducts)
                {
                    var pair = (supplier.Id, product.Id);
                    if (uniquePairs.Add(pair))
                    {
                        var sp = new SupplierProduct
                        {
                            SupplierId = supplier.Id,
                            ProductId = product.Id,
                            CreatedAt = DateTime.UtcNow.AddDays(-new Random().Next(1, 365)),
                            CreatedBy = new Random().Next(1, 10),
                            UpdatedAt = DateTime.UtcNow.AddDays(-new Random().Next(1, 30)),
                            UpdatedBy = new Random().Next(1, 10),
                            IsDeleted = false,
                            RowVersion = DateTime.UtcNow.AddDays(-new Random().Next(1, 10))
                        };
                        supplierProducts.Add(sp);
                    }
                }
            }

            await _context.SupplierProducts.AddRangeAsync(supplierProducts);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Seeded {supplierProducts.Count} supplier-product relationships");
        }

        private async Task SeedWarehouseDetailsAsync()
        {
            Console.WriteLine("Seeding warehouse details...");

            var warehouses = await _context.Warehouses.ToListAsync();

            var detailsFaker = new Faker<WarehouseDetail>()
                .RuleFor(wd => wd.WarehouseId, (f, wd) => 0) // Will be set manually
                .RuleFor(wd => wd.Address, f => f.Address.FullAddress())
                .RuleFor(wd => wd.Manager, f => f.Name.FullName())
                .RuleFor(wd => wd.CreatedAt, f => f.Date.Past(1))
                .RuleFor(wd => wd.CreatedBy, f => f.Random.Int(1, 10))
                .RuleFor(wd => wd.UpdatedAt, f => f.Date.Recent(30))
                .RuleFor(wd => wd.UpdatedBy, f => f.Random.Int(1, 10))
                .RuleFor(wd => wd.IsDeleted, f => false)
                .RuleFor(wd => wd.RowVersion, f => f.Date.Recent(10));

            var details = new List<WarehouseDetail>();

            foreach (var warehouse in warehouses)
            {
                var detail = detailsFaker.Generate();
                detail.WarehouseId = warehouse.Id;
                details.Add(detail);
            }

            await _context.WarehouseDetails.AddRangeAsync(details);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Seeded {details.Count} warehouse details");
        }

        public async Task ClearDatabaseAsync()
        {
            Console.WriteLine("Clearing database...");

            _context.SupplierProducts.RemoveRange(_context.SupplierProducts);
            _context.Inventory.RemoveRange(_context.Inventory);
            _context.WarehouseDetails.RemoveRange(_context.WarehouseDetails);
            _context.Products.RemoveRange(_context.Products);
            _context.Suppliers.RemoveRange(_context.Suppliers);
            _context.Warehouses.RemoveRange(_context.Warehouses);

            await _context.SaveChangesAsync();

            Console.WriteLine("Database cleared successfully!");
        }
    }

    // Extension method for easy usage in Program.cs
    public static class SeederExtensions
    {
        public static async Task SeedDatabaseAsync(this WarehouseDbContext context)
        {
            var seeder = new WarehouseSeedDb(context);
            await seeder.SeedAsync();
        }

        public static async Task ClearDatabaseAsync(this WarehouseDbContext context)
        {
            var seeder = new WarehouseSeedDb(context);
            await seeder.ClearDatabaseAsync();
        }
    
}