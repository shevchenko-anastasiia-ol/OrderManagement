using Microsoft.EntityFrameworkCore;
using WarehouseDAL.Data;
using WarehouseDAL.Interfaces;
using WarehouseDAL.Repositories.Interfaces;
using WarehouseDomain.Entities;

namespace WarehouseDAL.Repositories;

public class WarehouseRepository : GenericRepository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(WarehouseDbContext context) : base(context)
    {
    }

    public async Task<Warehouse?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(w => w.Details)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<Warehouse?> GetByIdWithInventoryAsync(int id)
    {
        return await _dbSet
            .Include(w => w.Inventories)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<IEnumerable<Warehouse>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(w => w.Details)
            .ToListAsync();
    }

    public async Task<IEnumerable<Warehouse>> GetWarehousesByCapacityAsync(int minCapacity)
    {
        return await _dbSet
            .Where(w => w.Capacity >= minCapacity)
            .ToListAsync();
    }
}