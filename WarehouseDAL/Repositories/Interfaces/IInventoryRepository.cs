using WarehouseDomain.Entities;

namespace WarehouseDAL.Repositories.Interfaces;

public interface IInventoryRepository : IGenericRepository<Inventory>
{
    Task<Inventory?> GetByWarehouseAndProductAsync(int warehouseId, int productId);
    Task<IEnumerable<Inventory>> GetByWarehouseIdAsync(int warehouseId);
    Task<IEnumerable<Inventory>> GetByProductIdAsync(int productId);
    Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold);
    Task<Inventory?> GetByIdWithDetailsAsync(int id);
}