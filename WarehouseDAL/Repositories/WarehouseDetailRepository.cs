using Microsoft.EntityFrameworkCore;
using WarehouseDAL.Data;
using WarehouseDAL.Interfaces;
using WarehouseDAL.Repositories.Interfaces;
using WarehouseDomain.Entities;

namespace WarehouseDAL.Repositories;

public class WarehouseDetailRepository : GenericRepository<WarehouseDetail>, IWarehouseDetailRepository
{
    public WarehouseDetailRepository(WarehouseDbContext context) : base(context)
    {
    }

    public async Task<WarehouseDetail?> GetByWarehouseIdAsync(int warehouseId)
    {
        return await _dbSet
            .Include(wd => wd.Warehouse)
            .FirstOrDefaultAsync(wd => wd.WarehouseId == warehouseId);
    }
}