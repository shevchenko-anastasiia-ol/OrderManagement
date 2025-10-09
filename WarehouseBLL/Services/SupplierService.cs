using AutoMapper;
using WarehouseBLL.DTOs.Product;
using WarehouseBLL.DTOs.Supplier;
using WarehouseBLL.Exceptions; 
using WarehouseBLL.Helpers;
using WarehouseBLL.Services.Interfaces;
using WarehouseBLL.Specifications.Supplier;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services;

public class SupplierService : ISupplierService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SupplierService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SupplierDto?> GetSupplierByIdAsync(int id)
    {
        var spec = new SupplierByIdSpec(id);
        var supplier = await _unitOfWork.SupplierRepository.SingleOrDefaultAsync(spec);
        
        if (supplier == null)
            throw new NotFoundException($"Supplier with ID {id} not found.");

        return _mapper.Map<SupplierDto>(supplier);
    }

    public async Task<SupplierWithProductDto?> GetSupplierWithProductsAsync(int id)
    {
        var spec = new SupplierByIdSpec(id, includeProducts: true);
        var supplier = await _unitOfWork.SupplierRepository.SingleOrDefaultAsync(spec);
        
        if (supplier == null)
            throw new NotFoundException($"Supplier with ID {id} not found.");

        return _mapper.Map<SupplierWithProductDto>(supplier);
    }

    public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
    {
        var queryParams = new SupplierQueryParams { Page = 1, PageSize = int.MaxValue };
        var spec = new SupplierSpecification(queryParams);
        var suppliers = await _unitOfWork.SupplierRepository.ListAsync(spec);
        
        if (!suppliers.Any())
            throw new NotFoundException("No suppliers found.");

        return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
    }

    public async Task<IEnumerable<SupplierDto>> GetSuppliersByCountryAsync(string country)
    {
        if (string.IsNullOrWhiteSpace(country))
            throw new BadRequestException("Country must be provided.");

        var spec = new SuppliersByCountrySpec(country);
        var suppliers = await _unitOfWork.SupplierRepository.ListAsync(spec);
        
        if (!suppliers.Any())
            throw new NotFoundException($"No suppliers found in country '{country}'.");

        return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
    }

    public async Task<IEnumerable<SupplierWithProductDto>> GetAllSuppliersWithProductsAsync()
    {
        var spec = new SuppliersWithProductsSpec();
        var suppliers = await _unitOfWork.SupplierRepository.ListAsync(spec);
        
        if (!suppliers.Any())
            throw new NotFoundException("No suppliers with products found.");

        return _mapper.Map<IEnumerable<SupplierWithProductDto>>(suppliers);
    }

    public async Task<SupplierDto> CreateSupplierAsync(SupplierCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new BadRequestException("Supplier name cannot be empty.");

        var exists = await _unitOfWork.SupplierRepository.AnyAsync(s => s.Name == dto.Name && !s.IsDeleted);
        if (exists)
            throw new ConflictException($"Supplier with name '{dto.Name}' already exists.");

        var supplier = _mapper.Map<Supplier>(dto);
        supplier.CreatedAt = DateTime.UtcNow;
        supplier.IsDeleted = false;

        await _unitOfWork.SupplierRepository.AddAsync(supplier);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<SupplierDto>(supplier);
    }

    public async Task<SupplierDto> UpdateSupplierAsync(SupplierUpdateDto dto)
    {
        var spec = new SupplierByIdSpec(dto.Id);
        var supplier = await _unitOfWork.SupplierRepository.SingleOrDefaultAsync(spec);
        
        if (supplier == null)
            throw new NotFoundException($"Supplier with ID {dto.Id} not found.");

        _mapper.Map(dto, supplier);
        supplier.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.SupplierRepository.Update(supplier);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<SupplierDto>(supplier);
    }

    public async Task DeleteSupplierAsync(int id)
    {
        var spec = new SupplierByIdSpec(id);
        var supplier = await _unitOfWork.SupplierRepository.SingleOrDefaultAsync(spec);
        
        if (supplier == null)
            throw new NotFoundException($"Supplier with ID {id} not found.");

        supplier.IsDeleted = true;
        supplier.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.SupplierRepository.Update(supplier);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> SupplierExistsAsync(int id)
    {
        return await _unitOfWork.SupplierRepository.AnyAsync(s => s.Id == id && !s.IsDeleted);
    }

    private SupplierDto MapToViewDto(Supplier supplier) => new()
    {
        Id = supplier.Id,
        Name = supplier.Name,
        Country = supplier.Country,
        ContactInfo = supplier.ContactInfo,
        CreatedAt = supplier.CreatedAt,
        UpdatedAt = supplier.UpdatedAt,
        IsDeleted = supplier.IsDeleted
    };

    private SupplierWithProductDto MapToWithProductsDto(Supplier supplier) => new()
    {
        Id = supplier.Id,
        Name = supplier.Name,
        Country = supplier.Country,
        ContactInfo = supplier.ContactInfo,
        CreatedAt = supplier.CreatedAt,
        UpdatedAt = supplier.UpdatedAt,
        IsDeleted = supplier.IsDeleted,
        Products = supplier.SupplierProducts.Select(sp => new ProductDto
        {
            Id = sp.Product.Id,
            Name = sp.Product.Name,
            SKU = sp.Product.SKU,
            Price = sp.Product.Price,
            CreatedAt = sp.Product.CreatedAt,
            UpdatedAt = sp.Product.UpdatedAt,
            IsDeleted = sp.Product.IsDeleted
        }).ToList()
    };

    public async Task<PagedResult<SupplierDto>> GetSuppliersPagedAsync(SupplierQueryParams queryParams)
    {
        var spec = new SupplierSpecification(queryParams);
        var countSpec = new SupplierCountSpecification(queryParams);

        var suppliers = await _unitOfWork.SupplierRepository.ListAsync(spec);
        var totalCount = await _unitOfWork.SupplierRepository.CountAsync(countSpec);

        var items = _mapper.Map<IEnumerable<SupplierDto>>(suppliers);

        return new PagedResult<SupplierDto>(items, totalCount, queryParams.Page, queryParams.PageSize);
    }
}
