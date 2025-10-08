using WarehouseBLL.DTOs.WarehouseDetail;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services.Interfaces;

public interface IWarehouseDetailService
{
    Task<WarehouseDetailDto?> GetWarehouseDetailsByIdAsync(int id);
    Task<WarehouseDetailDto?> GetWarehouseDetailsByWarehouseIdAsync(int warehouseId);
    Task<WarehouseDetailDto> CreateWarehouseDetailsAsync(WarehouseDetailCreateDto dto);
    Task<WarehouseDetailDto> UpdateWarehouseDetailsAsync(WarehouseDetailUpdateDto dto);
    Task DeleteWarehouseDetailsAsync(int id);
}