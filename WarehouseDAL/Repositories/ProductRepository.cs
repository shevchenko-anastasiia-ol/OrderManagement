using Microsoft.EntityFrameworkCore;
using WarehouseDAL.Data;
using WarehouseDAL.Interfaces;
using WarehouseDAL.Repositories.Interfaces;
using WarehouseDomain.Entities;

namespace WarehouseDAL.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(WarehouseDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetBySkuAsync(string sku)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.SKU == sku);
    }

    public async Task<Product?> GetByIdWithInventoryAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Inventories)
            .ThenInclude(i => i.Warehouse)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _dbSet
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsWithSuppliersAsync()
    {
        return await _dbSet
            .Include(p => p.SupplierProducts)
            .ThenInclude(sp => sp.Supplier)
            .ToListAsync();
    }
}