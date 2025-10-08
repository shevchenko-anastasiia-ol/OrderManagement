using WarehouseBLL.DTOs.SupplierProduct;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services.Interfaces;

public interface ISupplierProductService
{
    Task<SupplierProductDto?> GetSupplierProductByIdAsync(int id);
    Task<IEnumerable<SupplierProductDto>> GetSupplierProductsBySupplierAsync(int supplierId);
    Task<IEnumerable<SupplierProductDto>> GetSupplierProductsByProductAsync(int productId);
    Task<SupplierProductDto?> GetSupplierProductAsync(int supplierId, int productId);
    Task<SupplierProductDto> AddProductToSupplierAsync(SupplierProductCreateDto dto);
    Task RemoveProductFromSupplierAsync(int supplierId, int productId);
    Task<bool> SupplierProductExistsAsync(int supplierId, int productId);
}