using MarketplaceDAL.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MarketplaceDAL.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetByIdempotencyTokenAsync(string idempotencyToken);
        Task AddAsync(Product entity, IDbTransaction? transaction = null);
        Task UpdateAsync(Product entity, IDbTransaction? transaction = null);
        Task<Product?> GetByIdAsync(long id, IDbTransaction? transaction = null);
        Task<IEnumerable<Product>> GetAllAsync(IDbTransaction? transaction = null);
        Task DeleteAsync(long id, IDbTransaction? transaction = null);

        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, IDbTransaction? transaction = null);
        Task<IEnumerable<Product>> GetProductsInStockAsync(IDbTransaction? transaction = null);
        Task<IEnumerable<Product>> FindProductsByNameAsync(string name, IDbTransaction? transaction = null);
        Task<IEnumerable<Product>> GetAvailableProductsAsync(IDbTransaction? transaction = null);
        Task<Product?> GetProductWithOrderItemsAsync(long productId, IDbTransaction? transaction = null);
        Task<int> CountProductsInStockAsync(IDbTransaction? transaction = null);
        Task<List<string>> GetDistinctProductNamesAsync(IDbTransaction? transaction = null);
    }
}