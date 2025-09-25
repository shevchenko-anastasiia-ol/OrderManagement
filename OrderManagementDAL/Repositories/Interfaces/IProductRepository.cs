using MarketplaceDAL.Models;

namespace MarketplaceDAL.Repositories.Interfaces;

public interface IProductRepository :  IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetAvailableProductsAsync();
    
    Task<IEnumerable<Product>> FindProductsByNameAsync(string partialName);
    
    Task<Product> GetProductWithOrderItemsAsync(long productId);
}