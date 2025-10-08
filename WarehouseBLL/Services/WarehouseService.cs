using WarehouseBLL.DTOs.Inventory;
using WarehouseBLL.DTOs.Warehouse;
using WarehouseBLL.DTOs.WarehouseDetail;
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

        public async Task<WarehouseDto?> GetWarehouseByIdAsync(int id)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByIdAsync(id);
            return warehouse == null ? null : MapToViewDto(warehouse);
        }

        public async Task<WarehouseWithDetailDto?> GetWarehouseWithDetailsAsync(int id)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByIdWithDetailsAsync(id);
            return warehouse == null ? null : MapToWithDetailsDto(warehouse);
        }

        public async Task<WarehouseWithInventoryDto?> GetWarehouseWithInventoryAsync(int id)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByIdWithInventoryAsync(id);
            return warehouse == null ? null : MapToWithInventoryDto(warehouse);
        }

        public async Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync()
        {
            var warehouses = await _unitOfWork.WarehouseRepository.GetAllAsync();
            return warehouses.Select(MapToViewDto);
        }

        public async Task<IEnumerable<WarehouseWithDetailDto>> GetAllWarehousesWithDetailsAsync()
        {
            var warehouses = await _unitOfWork.WarehouseRepository.GetAllWithDetailsAsync();
            return warehouses.Select(MapToWithDetailsDto);
        }

        public async Task<IEnumerable<WarehouseDto>> GetWarehousesByMinCapacityAsync(int minCapacity)
        {
            var warehouses = await _unitOfWork.WarehouseRepository.GetWarehousesByCapacityAsync(minCapacity);
            return warehouses.Select(MapToViewDto);
        }

        public async Task<WarehouseDto> CreateWarehouseAsync(WarehouseCreateDto dto)
        {
            var warehouse = new Warehouse
            {
                Name = dto.Name,
                Capacity = dto.Capacity,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy,
                IsDeleted = false
            };

            await _unitOfWork.WarehouseRepository.AddAsync(warehouse);
            await _unitOfWork.SaveChangesAsync();

            return MapToViewDto(warehouse);
        }

        public async Task<WarehouseDto> UpdateWarehouseAsync(WarehouseUpdateDto dto)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByIdAsync(dto.Id);
            if (warehouse == null)
                throw new InvalidOperationException($"Warehouse with ID {dto.Id} not found.");

            warehouse.Name = dto.Name;
            warehouse.Capacity = dto.Capacity;
            warehouse.UpdatedAt = DateTime.UtcNow;
            warehouse.UpdatedBy = dto.UpdatedBy;

            _unitOfWork.WarehouseRepository.Update(warehouse);
            await _unitOfWork.SaveChangesAsync();

            return MapToViewDto(warehouse);
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

        private WarehouseDto MapToViewDto(Warehouse warehouse) => new()
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Capacity = warehouse.Capacity,
            CreatedAt = warehouse.CreatedAt,
            UpdatedAt = warehouse.UpdatedAt,
            IsDeleted = warehouse.IsDeleted
        };

        private WarehouseWithDetailDto MapToWithDetailsDto(Warehouse warehouse) => new()
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Capacity = warehouse.Capacity,
            CreatedAt = warehouse.CreatedAt,
            UpdatedAt = warehouse.UpdatedAt,
            IsDeleted = warehouse.IsDeleted,
            Details = warehouse.Details.FirstOrDefault() == null ? null : new WarehouseDetailDto
            {
                Id = warehouse.Details.First().Id,
                WarehouseId = warehouse.Details.First().WarehouseId,
                Address = warehouse.Details.First().Address,
                Manager = warehouse.Details.First().Manager,
                CreatedAt = warehouse.Details.First().CreatedAt,
                UpdatedAt = warehouse.Details.First().UpdatedAt,
                IsDeleted = warehouse.Details.First().IsDeleted
            }
        };

        private WarehouseWithInventoryDto MapToWithInventoryDto(Warehouse warehouse) => new()
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Capacity = warehouse.Capacity,
            CreatedAt = warehouse.CreatedAt,
            UpdatedAt = warehouse.UpdatedAt,
            IsDeleted = warehouse.IsDeleted,
            Inventories = warehouse.Inventories.Select(i => new InventoryDto
            {
                Id = i.Id,
                WarehouseId = i.WarehouseId,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                IsDeleted = i.IsDeleted
            }).ToList()
        };
}