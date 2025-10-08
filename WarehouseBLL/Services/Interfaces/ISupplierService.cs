using WarehouseDomain.Entities;

namespace WarehouseBLL.Services.Interfaces;

public interface ISupplierService
{
    Task<Supplier?> GetSupplierByIdAsync(int id);
    Task<Supplier?> GetSupplierWithProductsAsync(int id);
    Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
    Task<IEnumerable<Supplier>> GetSuppliersByCountryAsync(string country);
    Task<IEnumerable<Supplier>> GetAllSuppliersWithProductsAsync();
    Task<Supplier> CreateSupplierAsync(Supplier supplier);
    Task UpdateSupplierAsync(Supplier supplier);
    Task DeleteSupplierAsync(int id);
    Task<bool> SupplierExistsAsync(int id);
}