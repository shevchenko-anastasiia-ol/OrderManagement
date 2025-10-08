using WarehouseBLL.DTOs.Warehouse;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services.Interfaces;

public interface IWarehouseService
{
    Task<WarehouseDto?> GetWarehouseByIdAsync(int id);
    Task<WarehouseWithDetailDto?> GetWarehouseWithDetailsAsync(int id);
    Task<WarehouseWithInventoryDto?> GetWarehouseWithInventoryAsync(int id);
    Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync();
    Task<IEnumerable<WarehouseWithDetailDto>> GetAllWarehousesWithDetailsAsync();
    Task<IEnumerable<WarehouseDto>> GetWarehousesByMinCapacityAsync(int minCapacity);
    Task<WarehouseDto> CreateWarehouseAsync(WarehouseCreateDto dto);
    Task<WarehouseDto> UpdateWarehouseAsync(WarehouseUpdateDto dto);
    Task DeleteWarehouseAsync(int id);
    Task<bool> WarehouseExistsAsync(int id);
}