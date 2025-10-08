using WarehouseBLL.Services.Interfaces;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services;

public class WarehouseDetailService : IWarehouseDetailService
{
    private readonly IUnitOfWork _unitOfWork;

    public WarehouseDetailService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<WarehouseDetail?> GetWarehouseDetailsByIdAsync(int id)
    {
        return await _unitOfWork.WarehouseDetailRepository.GetByIdAsync(id);
    }

    public async Task<WarehouseDetail?> GetWarehouseDetailsByWarehouseIdAsync(int warehouseId)
    {
        return await _unitOfWork.WarehouseDetailRepository.GetByWarehouseIdAsync(warehouseId);
    }

    public async Task<WarehouseDetail> CreateWarehouseDetailsAsync(WarehouseDetail details)
    {
        var existing = await _unitOfWork.WarehouseDetailRepository.GetByWarehouseIdAsync(details.WarehouseId);
        if (existing != null)
        {
            throw new InvalidOperationException(
                $"Warehouse details already exist for Warehouse {details.WarehouseId}");
        }

        details.CreatedAt = DateTime.UtcNow;
        details.IsDeleted = false;
            
        await _unitOfWork.WarehouseDetailRepository.AddAsync(details);
        await _unitOfWork.SaveChangesAsync();
            
        return details;
    }

    public async Task UpdateWarehouseDetailsAsync(WarehouseDetail details)
    {
        details.UpdatedAt = DateTime.UtcNow;
            
        _unitOfWork.WarehouseDetailRepository.Update(details);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteWarehouseDetailsAsync(int id)
    {
        var details = await _unitOfWork.WarehouseDetailRepository.GetByIdAsync(id);
        if (details != null)
        {
            details.IsDeleted = true;
            details.UpdatedAt = DateTime.UtcNow;
                
            _unitOfWork.WarehouseDetailRepository.Update(details);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}