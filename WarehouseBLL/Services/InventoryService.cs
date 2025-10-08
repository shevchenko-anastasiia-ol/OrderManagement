using WarehouseBLL.Services.Interfaces;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;

        public InventoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Inventory?> GetInventoryByIdAsync(int id)
        {
            return await _unitOfWork.InventoryRepository.GetByIdAsync(id);
        }

        public async Task<Inventory?> GetInventoryWithDetailsAsync(int id)
        {
            return await _unitOfWork.InventoryRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<Inventory?> GetInventoryByWarehouseAndProductAsync(int warehouseId, int productId)
        {
            return await _unitOfWork.InventoryRepository.GetByWarehouseAndProductAsync(warehouseId, productId);
        }

        public async Task<IEnumerable<Inventory>> GetInventoryByWarehouseAsync(int warehouseId)
        {
            return await _unitOfWork.InventoryRepository.GetByWarehouseIdAsync(warehouseId);
        }

        public async Task<IEnumerable<Inventory>> GetInventoryByProductAsync(int productId)
        {
            return await _unitOfWork.InventoryRepository.GetByProductIdAsync(productId);
        }

        public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold)
        {
            return await _unitOfWork.InventoryRepository.GetLowStockItemsAsync(threshold);
        }

        public async Task<Inventory> CreateInventoryAsync(Inventory inventory)
        {
            var existing = await GetInventoryByWarehouseAndProductAsync(
                inventory.WarehouseId, 
                inventory.ProductId);

            if (existing != null)
            {
                throw new InvalidOperationException(
                    $"Inventory already exists for Warehouse {inventory.WarehouseId} and Product {inventory.ProductId}");
            }

            inventory.CreatedAt = DateTime.UtcNow;
            inventory.IsDeleted = false;
            
            await _unitOfWork.InventoryRepository.AddAsync(inventory);
            await _unitOfWork.SaveChangesAsync();
            
            return inventory;
        }

        public async Task UpdateInventoryAsync(Inventory inventory)
        {
            inventory.UpdatedAt = DateTime.UtcNow;
            
            _unitOfWork.InventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AdjustInventoryQuantityAsync(int id, int quantityChange)
        {
            var inventory = await _unitOfWork.InventoryRepository.GetByIdAsync(id);
            if (inventory == null)
            {
                throw new InvalidOperationException($"Inventory with ID {id} not found.");
            }

            inventory.Quantity += quantityChange;
            
            if (inventory.Quantity < 0)
            {
                throw new InvalidOperationException("Inventory quantity cannot be negative.");
            }

            inventory.UpdatedAt = DateTime.UtcNow;
            
            _unitOfWork.InventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteInventoryAsync(int id)
        {
            var inventory = await _unitOfWork.InventoryRepository.GetByIdAsync(id);
            if (inventory != null)
            {
                inventory.IsDeleted = true;
                inventory.UpdatedAt = DateTime.UtcNow;
                
                _unitOfWork.InventoryRepository.Update(inventory);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> InventoryExistsAsync(int id)
        {
            return await _unitOfWork.InventoryRepository.AnyAsync(i => i.Id == id && !i.IsDeleted);
        }

        public async Task<int> GetTotalStockForProductAsync(int productId)
        {
            var inventories = await _unitOfWork.InventoryRepository.GetByProductIdAsync(productId);
            return inventories.Sum(i => i.Quantity);
        }
}