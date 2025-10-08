using WarehouseBLL.DTOs.Product;
using WarehouseBLL.DTOs.Supplier;
using WarehouseBLL.Services.Interfaces;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services;

public class SupplierService : ISupplierService
{
    private readonly IUnitOfWork _unitOfWork;

        public SupplierService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SupplierDto?> GetSupplierByIdAsync(int id)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(id);
            return supplier == null ? null : MapToViewDto(supplier);
        }

        public async Task<SupplierWithProductDto?> GetSupplierWithProductsAsync(int id)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetByIdWithProductsAsync(id);
            return supplier == null ? null : MapToWithProductsDto(supplier);
        }

        public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
        {
            var suppliers = await _unitOfWork.SupplierRepository.GetAllAsync();
            return suppliers.Select(MapToViewDto);
        }

        public async Task<IEnumerable<SupplierDto>> GetSuppliersByCountryAsync(string country)
        {
            var suppliers = await _unitOfWork.SupplierRepository.GetSuppliersByCountryAsync(country);
            return suppliers.Select(MapToViewDto);
        }

        public async Task<IEnumerable<SupplierWithProductDto>> GetAllSuppliersWithProductsAsync()
        {
            var suppliers = await _unitOfWork.SupplierRepository.GetAllWithProductsAsync();
            return suppliers.Select(MapToWithProductsDto);
        }

        public async Task<SupplierDto> CreateSupplierAsync(SupplierCreateDto dto)
        {
            var supplier = new Supplier
            {
                Name = dto.Name,
                Country = dto.Country,
                ContactInfo = dto.ContactInfo,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy,
                IsDeleted = false
            };

            await _unitOfWork.SupplierRepository.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();

            return MapToViewDto(supplier);
        }

        public async Task<SupplierDto> UpdateSupplierAsync(SupplierUpdateDto dto)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(dto.Id);
            if (supplier == null)
                throw new InvalidOperationException($"Supplier with ID {dto.Id} not found.");

            supplier.Name = dto.Name;
            supplier.Country = dto.Country;
            supplier.ContactInfo = dto.ContactInfo;
            supplier.UpdatedAt = DateTime.UtcNow;
            supplier.UpdatedBy = dto.UpdatedBy;

            _unitOfWork.SupplierRepository.Update(supplier);
            await _unitOfWork.SaveChangesAsync();

            return MapToViewDto(supplier);
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
}