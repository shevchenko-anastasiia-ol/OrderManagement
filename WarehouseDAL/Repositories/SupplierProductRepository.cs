using Microsoft.EntityFrameworkCore;
using WarehouseDAL.Data;
using WarehouseDAL.Interfaces;
using WarehouseDAL.Repositories.Interfaces;
using WarehouseDomain.Entities;

namespace WarehouseDAL.Repositories;

public class SupplierProductRepository : GenericRepository<SupplierProduct>, ISupplierProductRepository
{
    public SupplierProductRepository(WarehouseDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<SupplierProduct>> GetBySupplierIdAsync(int supplierId)
    {
        return await _dbSet
            .Include(sp => sp.Product)
            .Where(sp => sp.SupplierId == supplierId)
            .ToListAsync();
    }

    public async Task<IEnumerable<SupplierProduct>> GetByProductIdAsync(int productId)
    {
        return await _dbSet
            .Include(sp => sp.Supplier)
            .Where(sp => sp.ProductId == productId)
            .ToListAsync();
    }

    public async Task<SupplierProduct?> GetBySupplierAndProductAsync(int supplierId, int productId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(sp => sp.SupplierId == supplierId && sp.ProductId == productId);
    }
}