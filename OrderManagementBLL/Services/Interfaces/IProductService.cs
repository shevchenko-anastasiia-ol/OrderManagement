using OrderManagementBLL.DTOs.Product;

namespace OrderManagementBLL.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto> GetByIdAsync(long id);
    Task<ProductDto> AddAsync(ProductCreateDto dto, string createdBy);
    Task<ProductDto> UpdateAsync(ProductUpdateDto dto, string updatedBy);
    Task DeleteAsync(long id);

    Task<IEnumerable<ProductDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<ProductDto>> GetProductsInStockAsync();
    Task<IEnumerable<ProductDto>> FindProductsByNameAsync(string name);
    Task<int> CountProductsInStockAsync();
    Task<List<string>> GetDistinctProductNamesAsync();
    Task<ProductDto> GetProductWithOrderItemsAsync(long productId);
}