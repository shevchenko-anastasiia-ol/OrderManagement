using MarketplaceDAL.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MarketplaceDAL.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetByIdempotencyTokenAsync(string idempotencyToken);
        Task AddAsync(Product entity, CancellationToken ct = default);
        Task UpdateAsync(Product entity, CancellationToken ct = default);
        Task<Product?> GetByIdAsync(long id, CancellationToken ct = default);
        Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct = default);
        Task DeleteAsync(long id, CancellationToken ct = default);

        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken ct = default);
        Task<IEnumerable<Product>> GetProductsInStockAsync(CancellationToken ct = default);
        Task<IEnumerable<Product>> FindProductsByNameAsync(string name, CancellationToken ct = default);
        Task<IEnumerable<Product>> GetAvailableProductsAsync(CancellationToken ct = default);
        Task<Product?> GetProductWithOrderItemsAsync(long productId, CancellationToken ct = default);
        Task<int> CountProductsInStockAsync(CancellationToken ct = default);
        Task<List<string>> GetDistinctProductNamesAsync(CancellationToken ct = default);
    }
}