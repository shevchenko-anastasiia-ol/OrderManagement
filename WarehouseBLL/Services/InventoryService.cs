using WarehouseBLL.DTOs.Inventory;
using WarehouseBLL.DTOs.Product;
using WarehouseBLL.DTOs.Warehouse;
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

        public async Task<InventoryDto?> GetInventoryByIdAsync(int id)
        {
            var inventory = await _unitOfWork.InventoryRepository.GetByIdAsync(id);
            return inventory == null ? null : MapToViewDto(inventory);
        }

        public async Task<InventoryWithDetailsDto?> GetInventoryWithDetailsAsync(int id)
        {
            var inventory = await _unitOfWork.InventoryRepository.GetByIdWithDetailsAsync(id);
            return inventory == null ? null : MapToWithDetailsDto(inventory);
        }

        public async Task<InventoryDto?> GetInventoryByWarehouseAndProductAsync(int warehouseId, int productId)
        {
            var inventory = await _unitOfWork.InventoryRepository.GetByWarehouseAndProductAsync(warehouseId, productId);
            return inventory == null ? null : MapToViewDto(inventory);
        }

        public async Task<IEnumerable<InventoryDto>> GetInventoryByWarehouseAsync(int warehouseId)
        {
            var inventories = await _unitOfWork.InventoryRepository.GetByWarehouseIdAsync(warehouseId);
            return inventories.Select(MapToViewDto);
        }

        public async Task<IEnumerable<InventoryDto>> GetInventoryByProductAsync(int productId)
        {
            var inventories = await _unitOfWork.InventoryRepository.GetByProductIdAsync(productId);
            return inventories.Select(MapToViewDto);
        }

        public async Task<IEnumerable<InventoryWithDetailsDto>> GetLowStockItemsAsync(int threshold)
        {
            var inventories = await _unitOfWork.InventoryRepository.GetLowStockItemsAsync(threshold);
            return inventories.Select(MapToWithDetailsDto);
        }

        public async Task<InventoryDto> CreateInventoryAsync(InventoryCreateDto dto)
        {
            var existing = await _unitOfWork.InventoryRepository.GetByWarehouseAndProductAsync(dto.WarehouseId, dto.ProductId);
            if (existing != null)
            {
                throw new InvalidOperationException(
                    $"Inventory already exists for Warehouse {dto.WarehouseId} and Product {dto.ProductId}");
            }

            var inventory = new Inventory
            {
                WarehouseId = dto.WarehouseId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy,
                IsDeleted = false
            };

            await _unitOfWork.InventoryRepository.AddAsync(inventory);
            await _unitOfWork.SaveChangesAsync();

            return MapToViewDto(inventory);
        }

        public async Task<InventoryDto> UpdateInventoryAsync(InventoryUpdateDto dto)
        {
            var inventory = await _unitOfWork.InventoryRepository.GetByIdAsync(dto.Id);
            if (inventory == null)
                throw new InvalidOperationException($"Inventory with ID {dto.Id} not found.");

            inventory.Quantity = dto.Quantity;
            inventory.UpdatedAt = DateTime.UtcNow;
            inventory.UpdatedBy = dto.UpdatedBy;

            _unitOfWork.InventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync();

            return MapToViewDto(inventory);
        }

        public async Task<InventoryDto> AdjustInventoryQuantityAsync(InventoryAdjustDto dto)
        {
            var inventory = await _unitOfWork.InventoryRepository.GetByIdAsync(dto.Id);
            if (inventory == null)
                throw new InvalidOperationException($"Inventory with ID {dto.Id} not found.");

            inventory.Quantity += dto.QuantityChange;

            if (inventory.Quantity < 0)
                throw new InvalidOperationException("Inventory quantity cannot be negative.");

            inventory.UpdatedAt = DateTime.UtcNow;
            inventory.UpdatedBy = dto.UpdatedBy;

            _unitOfWork.InventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync();

            return MapToViewDto(inventory);
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

        private InventoryDto MapToViewDto(Inventory inventory) => new()
        {
            Id = inventory.Id,
            WarehouseId = inventory.WarehouseId,
            ProductId = inventory.ProductId,
            Quantity = inventory.Quantity,
            CreatedAt = inventory.CreatedAt,
            UpdatedAt = inventory.UpdatedAt,
            IsDeleted = inventory.IsDeleted
        };

        private InventoryWithDetailsDto MapToWithDetailsDto(Inventory inventory) => new()
        {
            Id = inventory.Id,
            WarehouseId = inventory.WarehouseId,
            ProductId = inventory.ProductId,
            Quantity = inventory.Quantity,
            CreatedAt = inventory.CreatedAt,
            UpdatedAt = inventory.UpdatedAt,
            IsDeleted = inventory.IsDeleted,
            Warehouse = new WarehouseDto
            {
                Id = inventory.Warehouse.Id,
                Name = inventory.Warehouse.Name,
                Capacity = inventory.Warehouse.Capacity,
                CreatedAt = inventory.Warehouse.CreatedAt,
                UpdatedAt = inventory.Warehouse.UpdatedAt,
                IsDeleted = inventory.Warehouse.IsDeleted
            },
            Product = new ProductDto
            {
                Id = inventory.Product.Id,
                Name = inventory.Product.Name,
                SKU = inventory.Product.SKU,
                Price = inventory.Product.Price,
                CreatedAt = inventory.Product.CreatedAt,
                UpdatedAt = inventory.Product.UpdatedAt,
                IsDeleted = inventory.Product.IsDeleted
            }
        };
    }