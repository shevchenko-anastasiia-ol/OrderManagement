using AutoMapper;
using WarehouseBLL.DTOs.Inventory;
using WarehouseBLL.DTOs.Warehouse;
using WarehouseBLL.DTOs.WarehouseDetail;
using WarehouseBLL.Helpers;
using WarehouseBLL.Services.Interfaces;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services;

public class WarehouseService :  IWarehouseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

        public WarehouseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<WarehouseDto?> GetWarehouseByIdAsync(int id)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByIdAsync(id);
            return _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task<WarehouseWithDetailDto?> GetWarehouseWithDetailsAsync(int id)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<WarehouseWithDetailDto>(warehouse);
        }

        public async Task<WarehouseWithInventoryDto?> GetWarehouseWithInventoryAsync(int id)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByIdWithInventoryAsync(id);
            return _mapper.Map<WarehouseWithInventoryDto>(warehouse);
        }

        public async Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync()
        {
            var warehouses = await _unitOfWork.WarehouseRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
        }

        public async Task<IEnumerable<WarehouseWithDetailDto>> GetAllWarehousesWithDetailsAsync()
        {
            var warehouses = await _unitOfWork.WarehouseRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<WarehouseWithDetailDto>>(warehouses);
        }

        public async Task<IEnumerable<WarehouseDto>> GetWarehousesByMinCapacityAsync(int minCapacity)
        {
            var warehouses = await _unitOfWork.WarehouseRepository.GetWarehousesByCapacityAsync(minCapacity);
            return _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
        }

        public async Task<WarehouseDto> CreateWarehouseAsync(WarehouseCreateDto dto)
        {
            var warehouse = _mapper.Map<Warehouse>(dto);
            await _unitOfWork.WarehouseRepository.AddAsync(warehouse);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task<WarehouseDto> UpdateWarehouseAsync(WarehouseUpdateDto dto)
        {
            var warehouse = await _unitOfWork.WarehouseRepository.GetByIdAsync(dto.Id);
            if (warehouse == null)
                throw new InvalidOperationException($"Warehouse with ID {dto.Id} not found.");

            _mapper.Map(dto, warehouse);
            
            _unitOfWork.WarehouseRepository.Update(warehouse);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<WarehouseDto>(warehouse);
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

        public async Task<PagedResult<WarehouseDto>> GetWarehousesPagedAsync(WarehouseQueryParams queryParams)
        {
            var query = _unitOfWork.WarehouseRepository.GetAllAsync().Result.AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                query = query.Where(w => w.Name.Contains(queryParams.SearchTerm));
            }

            if (queryParams.MinCapacity.HasValue)
            {
                query = query.Where(w => w.Capacity >= queryParams.MinCapacity.Value);
            }

            if (queryParams.MaxCapacity.HasValue)
            {
                query = query.Where(w => w.Capacity <= queryParams.MaxCapacity.Value);
            }

            // Sorting
            query = query.ApplySorting(queryParams.SortBy ?? "Name", queryParams.SortDirection);

            // Pagination
            var pagedResult = await query.ToPagedResultAsync(queryParams.Page, queryParams.PageSize);

            return new PagedResult<WarehouseDto>(
                pagedResult.Items.Select(_mapper.Map<WarehouseDto>),
                pagedResult.TotalCount,
                pagedResult.Page,
                pagedResult.PageSize
            );
        }
}