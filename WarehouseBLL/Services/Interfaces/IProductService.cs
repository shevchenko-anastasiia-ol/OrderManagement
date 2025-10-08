using WarehouseBLL.DTOs.Product;
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
}