using WarehouseDomain.Entities;

namespace WarehouseBLL.Services.Interfaces;

public interface ISupplierProductService
{
    Task<SupplierProduct?> GetSupplierProductByIdAsync(int id);
    Task<IEnumerable<SupplierProduct>> GetSupplierProductsBySupplierAsync(int supplierId);
    Task<IEnumerable<SupplierProduct>> GetSupplierProductsByProductAsync(int productId);
    Task<SupplierProduct?> GetSupplierProductAsync(int supplierId, int productId);
    Task<SupplierProduct> AddProductToSupplierAsync(int supplierId, int productId);
    Task RemoveProductFromSupplierAsync(int supplierId, int productId);
    Task<bool> SupplierProductExistsAsync(int supplierId, int productId);
}