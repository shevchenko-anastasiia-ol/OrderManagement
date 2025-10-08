using Microsoft.EntityFrameworkCore;
using WarehouseDAL.Data;
using WarehouseDAL.Interfaces;
using WarehouseDAL.Repositories.Interfaces;
using WarehouseDomain.Entities;

namespace WarehouseDAL.Repositories;

public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
{
    public InventoryRepository(WarehouseDbContext context) : base(context)
    {
    }

    public async Task<Inventory?> GetByWarehouseAndProductAsync(int warehouseId, int productId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(i => i.WarehouseId == warehouseId && i.ProductId == productId);
    }

    public async Task<IEnumerable<Inventory>> GetByWarehouseIdAsync(int warehouseId)
    {
        return await _dbSet
            .Include(i => i.Product)
            .Where(i => i.WarehouseId == warehouseId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Inventory>> GetByProductIdAsync(int productId)
    {
        return await _dbSet
            .Include(i => i.Warehouse)
            .Where(i => i.ProductId == productId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold)
    {
        return await _dbSet
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .Where(i => i.Quantity <= threshold)
            .ToListAsync();
    }

    public async Task<Inventory?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(i => i.Warehouse)
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.Id == id);
    }
}