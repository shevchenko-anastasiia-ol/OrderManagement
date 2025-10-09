using AutoMapper;
using WarehouseBLL.DTOs.Inventory;
using WarehouseBLL.DTOs.Warehouse;
using WarehouseBLL.DTOs.WarehouseDetail;
using WarehouseBLL.Exceptions;
using WarehouseBLL.Helpers;
using WarehouseBLL.Services.Interfaces;
using WarehouseBLL.Specifications.Warehouse;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services;

public class WarehouseService : IWarehouseService
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
        var spec = new WarehouseByIdSpec(id);
        var warehouse = await _unitOfWork.WarehouseRepository.SingleOrDefaultAsync(spec);
        
        if (warehouse == null)
            throw new NotFoundException($"Warehouse with ID {id} not found.");

        return _mapper.Map<WarehouseDto>(warehouse);
    }

    public async Task<WarehouseWithDetailDto?> GetWarehouseWithDetailsAsync(int id)
    {
        var spec = new WarehouseByIdSpec(id, includeDetails: true);
        var warehouse = await _unitOfWork.WarehouseRepository.SingleOrDefaultAsync(spec);
        
        if (warehouse == null)
            throw new NotFoundException($"Warehouse with ID {id} not found.");

        return _mapper.Map<WarehouseWithDetailDto>(warehouse);
    }

    public async Task<WarehouseWithInventoryDto?> GetWarehouseWithInventoryAsync(int id)
    {
        var spec = new WarehouseByIdSpec(id, includeInventory: true);
        var warehouse = await _unitOfWork.WarehouseRepository.SingleOrDefaultAsync(spec);
        
        if (warehouse == null)
            throw new NotFoundException($"Warehouse with ID {id} not found.");

        return _mapper.Map<WarehouseWithInventoryDto>(warehouse);
    }

    public async Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync()
    {
        var queryParams = new WarehouseQueryParams { Page = 1, PageSize = int.MaxValue };
        var spec = new WarehouseSpecification(queryParams);
        var warehouses = await _unitOfWork.WarehouseRepository.ListAsync(spec);
        
        return _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
    }

    public async Task<IEnumerable<WarehouseWithDetailDto>> GetAllWarehousesWithDetailsAsync()
    {
        var spec = new WarehousesWithDetailsSpec();
        var warehouses = await _unitOfWork.WarehouseRepository.ListAsync(spec);
        
        return _mapper.Map<IEnumerable<WarehouseWithDetailDto>>(warehouses);
    }

    public async Task<IEnumerable<WarehouseDto>> GetWarehousesByMinCapacityAsync(int minCapacity)
    {
        if (minCapacity < 0)
            throw new BadRequestException("Minimum capacity must be a positive number.");

        var queryParams = new WarehouseQueryParams 
        { 
            MinCapacity = minCapacity,
            Page = 1,
            PageSize = int.MaxValue
        };
        var spec = new WarehouseSpecification(queryParams);
        var warehouses = await _unitOfWork.WarehouseRepository.ListAsync(spec);
        
        return _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
    }

    public async Task<WarehouseDto> CreateWarehouseAsync(WarehouseCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new BadRequestException("Warehouse name cannot be empty.");

        var warehouse = _mapper.Map<Warehouse>(dto);
        warehouse.CreatedAt = DateTime.UtcNow;
        warehouse.IsDeleted = false;

        await _unitOfWork.WarehouseRepository.AddAsync(warehouse);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<WarehouseDto>(warehouse);
    }

    public async Task<WarehouseDto> UpdateWarehouseAsync(WarehouseUpdateDto dto)
    {
        var spec = new WarehouseByIdSpec(dto.Id);
        var warehouse = await _unitOfWork.WarehouseRepository.SingleOrDefaultAsync(spec);
        
        if (warehouse == null)
            throw new NotFoundException($"Warehouse with ID {dto.Id} not found.");

        _mapper.Map(dto, warehouse);
        warehouse.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.WarehouseRepository.Update(warehouse);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<WarehouseDto>(warehouse);
    }

    public async Task DeleteWarehouseAsync(int id)
    {
        var spec = new WarehouseByIdSpec(id);
        var warehouse = await _unitOfWork.WarehouseRepository.SingleOrDefaultAsync(spec);
        
        if (warehouse == null)
            throw new NotFoundException($"Warehouse with ID {id} not found.");

        warehouse.IsDeleted = true;
        warehouse.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.WarehouseRepository.Update(warehouse);
        await _unitOfWork.SaveChangesAsync();
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
        var spec = new WarehouseSpecification(queryParams);
        var countSpec = new WarehouseCountSpecification(queryParams);

        var warehouses = await _unitOfWork.WarehouseRepository.ListAsync(spec);
        var totalCount = await _unitOfWork.WarehouseRepository.CountAsync(countSpec);

        var items = _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);

        return new PagedResult<WarehouseDto>(items, totalCount, queryParams.Page, queryParams.PageSize);
    }
}
