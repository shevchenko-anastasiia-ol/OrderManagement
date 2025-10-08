using WarehouseDomain.Entities;

namespace WarehouseDAL.Repositories.Interfaces;

public interface IWarehouseDetailRepository : IGenericRepository<WarehouseDetail>
{
    Task<WarehouseDetail?> GetByWarehouseIdAsync(int warehouseId);
}