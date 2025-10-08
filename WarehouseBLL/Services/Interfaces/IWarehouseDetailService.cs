using WarehouseDomain.Entities;

namespace WarehouseBLL.Services.Interfaces;

public interface IWarehouseDetailService
{
    Task<WarehouseDetail?> GetWarehouseDetailsByIdAsync(int id);
    Task<WarehouseDetail?> GetWarehouseDetailsByWarehouseIdAsync(int warehouseId);
    Task<WarehouseDetail> CreateWarehouseDetailsAsync(WarehouseDetail details);
    Task UpdateWarehouseDetailsAsync(WarehouseDetail details);
    Task DeleteWarehouseDetailsAsync(int id);
}