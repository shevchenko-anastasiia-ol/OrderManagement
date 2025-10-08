using WarehouseBLL.Services.Interfaces;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services;

public class WarehouseService :  IWarehouseService
{
    private readonly IUnitOfWork _unitOfWork;

        public WarehouseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Warehouse?> GetWarehouseByIdAsync(int id)
        {
            return await _unitOfWork.WarehouseRepository.GetByIdAsync(id);
        }

        public async Task<Warehouse?> GetWarehouseWithDetailsAsync(int id)
        {
            return await _unitOfWork.WarehouseRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<Warehouse?> GetWarehouseWithInventoryAsync(int id)
        {
            return await _unitOfWork.WarehouseRepository.GetByIdWithInventoryAsync(id);
        }

        public async Task<IEnumerable<Warehouse>> GetAllWarehousesAsync()
        {
            return await _unitOfWork.WarehouseRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Warehouse>> GetAllWarehousesWithDetailsAsync()
        {
            return await _unitOfWork.WarehouseRepository.GetAllWithDetailsAsync();
        }

        public async Task<IEnumerable<Warehouse>> GetWarehousesByMinCapacityAsync(int minCapacity)
        {
            return await _unitOfWork.WarehouseRepository.GetWarehousesByCapacityAsync(minCapacity);
        }

        public async Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse)
        {
            warehouse.CreatedAt = DateTime.UtcNow;
            warehouse.IsDeleted = false;
            
            await _unitOfWork.WarehouseRepository.AddAsync(warehouse);
            await _unitOfWork.SaveChangesAsync();
            
            return warehouse;
        }

        public async Task UpdateWarehouseAsync(Warehouse warehouse)
        {
            warehouse.UpdatedAt = DateTime.UtcNow;
            
            _unitOfWork.WarehouseRepository.Update(warehouse);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteWarehouseAsync(int id)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByIdAsync(id);
            if (warehouse != null)
            {
                warehouse.IsDeleted = true;
                warehouse.UpdatedAt = DateTime.UtcNow;
                
                _unitOfWork.WarehouseRepository.Update(warehouse);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> WarehouseExistsAsync(int id)
        {
            return await _unitOfWork.WarehouseRepository.AnyAsync(w => w.Id == id && !w.IsDeleted);
        }
}