using AutoMapper;
using WarehouseBLL.DTOs.Product;
using WarehouseBLL.DTOs.Supplier;
using WarehouseBLL.Helpers;
using WarehouseBLL.Services.Interfaces;
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
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(id);
            return _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<SupplierWithProductDto?> GetSupplierWithProductsAsync(int id)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetByIdWithProductsAsync(id);
            return _mapper.Map<SupplierWithProductDto>(supplier);
        }

        public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
        {
            var suppliers = await _unitOfWork.SupplierRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
        }

        public async Task<IEnumerable<SupplierDto>> GetSuppliersByCountryAsync(string country)
        {
            var suppliers = await _unitOfWork.SupplierRepository.GetSuppliersByCountryAsync(country);
            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
        }

        public async Task<IEnumerable<SupplierWithProductDto>> GetAllSuppliersWithProductsAsync()
        {
            var suppliers = await _unitOfWork.SupplierRepository.GetAllWithProductsAsync();
            return _mapper.Map<IEnumerable<SupplierWithProductDto>>(suppliers);
        }

        public async Task<SupplierDto> CreateSupplierAsync(SupplierCreateDto dto)
        {
            var supplier = _mapper.Map<Supplier>(dto);

            await _unitOfWork.SupplierRepository.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();

            return MapToViewDto(supplier);
        }

        public async Task<SupplierDto> UpdateSupplierAsync(SupplierUpdateDto dto)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(dto.Id);
            if (supplier == null)
                throw new InvalidOperationException($"Supplier with ID {dto.Id} not found.");

            _mapper.Map(dto, supplier);

            _unitOfWork.SupplierRepository.Update(supplier);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SupplierDto>(supplier);
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(id);
            if (supplier != null)
            {
                supplier.IsDeleted = true;
                supplier.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.SupplierRepository.Update(supplier);
                await _unitOfWork.SaveChangesAsync();
            }
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
            var query = _unitOfWork.SupplierRepository.GetAllAsync().Result.AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                query = query.Where(s => s.Name.Contains(queryParams.SearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.Country))
            {
                query = query.Where(s => s.Country == queryParams.Country);
            }

            // Sorting
            query = query.ApplySorting(queryParams.SortBy ?? "Name", queryParams.SortDirection);

            // Pagination
            var pagedResult = await query.ToPagedResultAsync(queryParams.Page, queryParams.PageSize);

            return new PagedResult<SupplierDto>(
                pagedResult.Items.Select(_mapper.Map<SupplierDto>),
                pagedResult.TotalCount,
                pagedResult.Page,
                pagedResult.PageSize
            );
        }
}