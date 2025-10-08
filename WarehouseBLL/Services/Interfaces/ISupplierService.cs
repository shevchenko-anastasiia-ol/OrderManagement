using WarehouseBLL.DTOs.Supplier;
using WarehouseBLL.Helpers;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services.Interfaces;

public interface ISupplierService
{
    Task<SupplierDto?> GetSupplierByIdAsync(int id);
    Task<SupplierWithProductDto?> GetSupplierWithProductsAsync(int id);
    Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync();
    Task<IEnumerable<SupplierDto>> GetSuppliersByCountryAsync(string country);
    Task<IEnumerable<SupplierWithProductDto>> GetAllSuppliersWithProductsAsync();
    Task<SupplierDto> CreateSupplierAsync(SupplierCreateDto dto);
    Task<SupplierDto> UpdateSupplierAsync(SupplierUpdateDto dto);
    Task DeleteSupplierAsync(int id);
    Task<bool> SupplierExistsAsync(int id);
    Task<PagedResult<SupplierDto>> GetSuppliersPagedAsync(SupplierQueryParams queryParams);
}