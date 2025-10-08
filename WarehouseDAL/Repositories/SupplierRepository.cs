using Microsoft.EntityFrameworkCore;
using WarehouseDAL.Data;
using WarehouseDAL.Interfaces;
using WarehouseDAL.Repositories.Interfaces;
using WarehouseDomain.Entities;

namespace WarehouseDAL.Repositories;

public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
{
    public SupplierRepository(WarehouseDbContext context) : base(context)
    {
    }

    public async Task<Supplier?> GetByIdWithProductsAsync(int id)
    {
        return await _dbSet
            .Include(s => s.SupplierProducts)
            .ThenInclude(sp => sp.Product)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Supplier>> GetSuppliersByCountryAsync(string country)
    {
        return await _dbSet
            .Where(s => s.Country == country)
            .ToListAsync();
    }

    public async Task<IEnumerable<Supplier>> GetAllWithProductsAsync()
    {
        return await _dbSet
            .Include(s => s.SupplierProducts)
            .ThenInclude(sp => sp.Product)
            .ToListAsync();
    }
}