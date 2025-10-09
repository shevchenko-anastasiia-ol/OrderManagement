using WarehouseBLL.DTOs.Product;
using WarehouseBLL.Helpers;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto?> GetProductBySkuAsync(string sku);
    Task<ProductWithInventoryDto?> GetProductWithInventoryAsync(int id);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<IEnumerable<ProductDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<ProductWithSuppliersDto>> GetProductsWithSuppliersAsync();
    Task<ProductDto> CreateProductAsync(ProductCreateDto dto);
    Task<ProductDto> UpdateProductAsync(ProductUpdateDto dto);
    Task DeleteProductAsync(int id);
    Task<bool> ProductExistsAsync(int id);
    Task<bool> SkuExistsAsync(string sku);
    Task<PagedResult<ProductDto>> GetProductsPagedAsync(ProductQueryParams queryParams);
    Task<ProductWithSuppliersDto?> GetProductWithSuppliersAsync(int id);
    Task<List<ProductDto>> GetLowStockProductsAsync(int threshold = 10);
    Task<List<ProductDto>> GetRecentProductsAsync(int count = 10);
}