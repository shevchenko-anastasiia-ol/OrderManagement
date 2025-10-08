using WarehouseDomain.Entities;

namespace WarehouseBLL.Services.Interfaces;

public interface IWarehouseService
{
    Task<Warehouse?> GetWarehouseByIdAsync(int id);
    Task<Warehouse?> GetWarehouseWithDetailsAsync(int id);
    Task<Warehouse?> GetWarehouseWithInventoryAsync(int id);
    Task<IEnumerable<Warehouse>> GetAllWarehousesAsync();
    Task<IEnumerable<Warehouse>> GetAllWarehousesWithDetailsAsync();
    Task<IEnumerable<Warehouse>> GetWarehousesByMinCapacityAsync(int minCapacity);
    Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse);
    Task UpdateWarehouseAsync(Warehouse warehouse);
    Task DeleteWarehouseAsync(int id);
    Task<bool> WarehouseExistsAsync(int id);
}