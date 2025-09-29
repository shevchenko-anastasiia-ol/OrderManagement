using MarketplaceDAL.Models;

namespace MarketplaceDAL.Repositories.Interfaces;

public interface IProductRepository :  IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetAvailableProductsAsync();
    
    Task<IEnumerable<Product>> FindProductsByNameAsync(string partialName);
    
    Task<Product> GetProductWithOrderItemsAsync(long productId);
    Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<Product>> GetProductsInStockAsync();
    Task<int> CountProductsInStockAsync();
    Task<List<string>> GetDistinctProductNamesAsync();

}