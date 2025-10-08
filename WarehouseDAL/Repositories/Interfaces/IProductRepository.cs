using WarehouseDomain.Entities;

namespace WarehouseDAL.Repositories.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetBySkuAsync(string sku);
    Task<Product?> GetByIdWithInventoryAsync(int id);
    Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<Product>> GetProductsWithSuppliersAsync();
}