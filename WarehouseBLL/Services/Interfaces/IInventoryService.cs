using WarehouseBLL.DTOs.Inventory;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services.Interfaces;

public interface IInventoryService
{
    Task<InventoryDto?> GetInventoryByIdAsync(int id);
    Task<InventoryWithDetailsDto?> GetInventoryWithDetailsAsync(int id);
    Task<InventoryDto?> GetInventoryByWarehouseAndProductAsync(int warehouseId, int productId);
    Task<IEnumerable<InventoryDto>> GetInventoryByWarehouseAsync(int warehouseId);
    Task<IEnumerable<InventoryDto>> GetInventoryByProductAsync(int productId);
    Task<IEnumerable<InventoryWithDetailsDto>> GetLowStockItemsAsync(int threshold);
    Task<InventoryDto> CreateInventoryAsync(InventoryCreateDto dto);
    Task<InventoryDto> UpdateInventoryAsync(InventoryUpdateDto dto);
    Task<InventoryDto> AdjustInventoryQuantityAsync(InventoryAdjustDto dto);
    Task DeleteInventoryAsync(int id);
    Task<bool> InventoryExistsAsync(int id);
    Task<int> GetTotalStockForProductAsync(int productId);
}