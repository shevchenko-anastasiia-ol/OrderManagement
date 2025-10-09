using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WarehouseBLL.DTOs.Inventory;
using WarehouseBLL.DTOs.Product;
using WarehouseBLL.DTOs.Warehouse;
using WarehouseBLL.Exceptions;
using WarehouseBLL.Helpers;
using WarehouseBLL.Services.Interfaces;
using WarehouseBLL.Specifications;
using WarehouseBLL.Specifications.Inventory;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services;

public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper  _mapper;

        public InventoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<InventoryDto?> GetInventoryByIdAsync(int id)
        {
            var spec = new InventoryByIdSpecification(id);
            var inventory = await _unitOfWork.InventoryRepository.SingleOrDefaultAsync(spec);
        
            if (inventory == null)
                throw new NotFoundException($"Inventory with ID {id} not found.");

            return _mapper.Map<InventoryDto>(inventory);
        }

        public async Task<InventoryWithDetailsDto?> GetInventoryWithDetailsAsync(int id)
        {
            
            var spec = new InventoryByIdSpecification(id);
            var inventory = await _unitOfWork.InventoryRepository.SingleOrDefaultAsync(spec);
        
            if (inventory == null)
                throw new NotFoundException($"Inventory with ID {id} not found.");

            return _mapper.Map<InventoryWithDetailsDto>(inventory);
        }

        public async Task<InventoryDto?> GetInventoryByWarehouseAndProductAsync(int warehouseId, int productId)
        {
            var spec = new InventoryByWarehouseAndProductSpec(warehouseId, productId);
            var inventory = await _unitOfWork.InventoryRepository.SingleOrDefaultAsync(spec);
        
            if (inventory == null)
                throw new NotFoundException($"No inventory found for Warehouse {warehouseId} and Product {productId}.");

            return _mapper.Map<InventoryDto>(inventory);
        }

        public async Task<IEnumerable<InventoryDto>> GetInventoryByWarehouseAsync(int warehouseId)
        {
            var spec = new InventoryByWarehouseSpec(warehouseId);
            var inventories = await _unitOfWork.InventoryRepository.ListAsync(spec);
        
            if (!inventories.Any())
                throw new NotFoundException($"No inventory records found for Warehouse ID {warehouseId}.");

            return _mapper.Map<IEnumerable<InventoryDto>>(inventories);
        }

        public async Task<IEnumerable<InventoryDto>> GetInventoryByProductAsync(int productId)
        {
            var spec = new InventoryByProductSpec(productId);
            var inventories = await _unitOfWork.InventoryRepository.ListAsync(spec);
        
            if (!inventories.Any())
                throw new NotFoundException($"No inventory records found for Product ID {productId}.");

            return _mapper.Map<IEnumerable<InventoryDto>>(inventories);
        }

        public async Task<IEnumerable<InventoryWithDetailsDto>> GetLowStockItemsAsync(int threshold)
        {
            var queryParams = new InventoryQueryParams 
            { 
                LowStock = true, 
                LowStockThreshold = threshold,
                Page = 1,
                PageSize = int.MaxValue
            };
            var spec = new InventorySpecification(queryParams);
            var inventories = await _unitOfWork.InventoryRepository.ListAsync(spec);
        
            return _mapper.Map<IEnumerable<InventoryWithDetailsDto>>(inventories);
        }

        public async Task<InventoryDto> CreateInventoryAsync(InventoryCreateDto dto)
        {
            var existingSpec = new InventoryByWarehouseAndProductSpec(dto.WarehouseId, dto.ProductId);
            var existing = await _unitOfWork.InventoryRepository.FirstOrDefaultAsync(existingSpec);
        
            if (existing != null)
                throw new ConflictException($"Inventory already exists for Warehouse {dto.WarehouseId} and Product {dto.ProductId}.");

            if (dto.Quantity < 0)
                throw new BadRequestException("Inventory quantity cannot be negative.");

            var inventory = _mapper.Map<Inventory>(dto);
            inventory.CreatedAt = DateTime.UtcNow;
            inventory.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.InventoryRepository.AddAsync(inventory);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<InventoryDto>(inventory);
        }

        public async Task<InventoryDto> UpdateInventoryAsync(InventoryUpdateDto dto)
        {
            var spec = new InventoryByIdSpecification(dto.Id);
            var inventory = await _unitOfWork.InventoryRepository.SingleOrDefaultAsync(spec);
        
            if (inventory == null)
                throw new NotFoundException($"Inventory with ID {dto.Id} not found.");

            if (dto.Quantity < 0)
                throw new BadRequestException("Inventory quantity cannot be negative.");

            _mapper.Map(dto, inventory);
            inventory.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.InventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<InventoryDto>(inventory);
        }

        public async Task<InventoryDto> AdjustInventoryQuantityAsync(InventoryAdjustDto dto)
        {
            var spec = new InventoryByIdSpecification(dto.Id);
            var inventory = await _unitOfWork.InventoryRepository.SingleOrDefaultAsync(spec);
        
            if (inventory == null)
                throw new NotFoundException($"Inventory with ID {dto.Id} not found.");

            inventory.Quantity += dto.QuantityChange;

            if (inventory.Quantity < 0)
                throw new BadRequestException("Inventory quantity cannot be negative.");

            inventory.UpdatedAt = DateTime.UtcNow;
            inventory.UpdatedBy = dto.UpdatedBy;

            _unitOfWork.InventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<InventoryDto>(inventory);
        }

        public async Task DeleteInventoryAsync(int id)
        {
            var spec = new InventoryByIdSpecification(id);
            var inventory = await _unitOfWork.InventoryRepository.SingleOrDefaultAsync(spec);
        
            if (inventory == null)
                throw new NotFoundException($"Inventory with ID {id} not found.");

            inventory.IsDeleted = true;
            inventory.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.InventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> InventoryExistsAsync(int id)
        {
            var spec = new InventoryByIdSpecification(id);
            return await _unitOfWork.InventoryRepository.AnyAsync(w => w.Id == id && !w.IsDeleted);
        }
        
        public async Task<int> GetTotalStockForProductAsync(int productId)
        {
            var spec = new InventoryByProductSpec(productId);
            var inventories = await _unitOfWork.InventoryRepository.ListAsync(spec);
        
            if (!inventories.Any())
                throw new NotFoundException($"No inventory found for Product ID {productId}.");

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
        
        public async Task<PagedResult<InventoryWithDetailsDto>> GetInventoryPagedAsync(InventoryQueryParams queryParams)
        {
            var spec = new InventorySpecification(queryParams);
            var countSpec = new InventoryCountSpecification(queryParams);

            var inventories = await _unitOfWork.InventoryRepository.ListAsync(spec);
            var totalCount = await _unitOfWork.InventoryRepository.CountAsync(countSpec);

            var items = _mapper.Map<IEnumerable<InventoryWithDetailsDto>>(inventories);

            return new PagedResult<InventoryWithDetailsDto>(items, totalCount, queryParams.Page, queryParams.PageSize);
        }

    }