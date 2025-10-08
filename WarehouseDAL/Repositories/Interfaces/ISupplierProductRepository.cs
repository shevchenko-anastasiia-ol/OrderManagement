using WarehouseDomain.Entities;

namespace WarehouseDAL.Repositories.Interfaces;

public interface ISupplierProductRepository : IGenericRepository<SupplierProduct>
{
    Task<IEnumerable<SupplierProduct>> GetBySupplierIdAsync(int supplierId);
    Task<IEnumerable<SupplierProduct>> GetByProductIdAsync(int productId);
    Task<SupplierProduct?> GetBySupplierAndProductAsync(int supplierId, int productId);
}