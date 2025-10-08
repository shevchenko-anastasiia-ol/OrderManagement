using WarehouseDomain.Entities;

namespace WarehouseBLL.Services.Interfaces;

public interface IInventoryService
{
    Task<Inventory?> GetInventoryByIdAsync(int id);
    Task<Inventory?> GetInventoryWithDetailsAsync(int id);
    Task<Inventory?> GetInventoryByWarehouseAndProductAsync(int warehouseId, int productId);
    Task<IEnumerable<Inventory>> GetInventoryByWarehouseAsync(int warehouseId);
    Task<IEnumerable<Inventory>> GetInventoryByProductAsync(int productId);
    Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold);
    Task<Inventory> CreateInventoryAsync(Inventory inventory);
    Task UpdateInventoryAsync(Inventory inventory);
    Task AdjustInventoryQuantityAsync(int id, int quantityChange);
    Task DeleteInventoryAsync(int id);
    Task<bool> InventoryExistsAsync(int id);
    Task<int> GetTotalStockForProductAsync(int productId);
}