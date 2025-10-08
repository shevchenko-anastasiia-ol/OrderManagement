using WarehouseDomain.Entities;

namespace WarehouseDAL.Repositories.Interfaces;

public interface IWarehouseRepository :  IGenericRepository<Warehouse>
{
    Task<Warehouse?> GetByIdWithDetailsAsync(int id);
    Task<Warehouse?> GetByIdWithInventoryAsync(int id);
    Task<IEnumerable<Warehouse>> GetAllWithDetailsAsync();
    Task<IEnumerable<Warehouse>> GetWarehousesByCapacityAsync(int minCapacity);
}