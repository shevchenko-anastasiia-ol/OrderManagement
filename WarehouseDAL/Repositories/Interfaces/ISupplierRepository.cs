using WarehouseDomain.Entities;

namespace WarehouseDAL.Repositories.Interfaces;

public interface ISupplierRepository : IGenericRepository<Supplier>
{
    Task<Supplier?> GetByIdWithProductsAsync(int id);
    Task<IEnumerable<Supplier>> GetSuppliersByCountryAsync(string country);
    Task<IEnumerable<Supplier>> GetAllWithProductsAsync();
}