using AutoMapper;
using WarehouseBLL.DTOs.SupplierProduct;
using WarehouseBLL.Services.Interfaces;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services;

public class SupplierProductService :  ISupplierProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

        public SupplierProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SupplierProductDto?> GetSupplierProductByIdAsync(int id)
        {
            var supplierProduct = await _unitOfWork.SupplierProductRepository.GetByIdAsync(id);
            return _mapper.Map<SupplierProductDto>(supplierProduct);
        }

        public async Task<IEnumerable<SupplierProductDto>> GetSupplierProductsBySupplierAsync(int supplierId)
        {
            var supplierProducts = await _unitOfWork.SupplierProductRepository.GetBySupplierIdAsync(supplierId);
            return _mapper.Map<IEnumerable<SupplierProductDto>>(supplierProducts);
        }

        public async Task<IEnumerable<SupplierProductDto>> GetSupplierProductsByProductAsync(int productId)
        {
            var supplierProducts = await _unitOfWork.SupplierProductRepository.GetByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<SupplierProductDto>>(supplierProducts);
        }

        public async Task<SupplierProductDto?> GetSupplierProductAsync(int supplierId, int productId)
        {
            var supplierProduct = await _unitOfWork.SupplierProductRepository.GetBySupplierAndProductAsync(supplierId, productId);
            return _mapper.Map<SupplierProductDto>(supplierProduct);
        }

        public async Task<SupplierProductDto> AddProductToSupplierAsync(SupplierProductCreateDto dto)
        {
            if (await SupplierProductExistsAsync(dto.SupplierId, dto.ProductId))
            {
                throw new InvalidOperationException(
                    $"Supplier {dto.SupplierId} already supplies Product {dto.ProductId}");
            }

            var supplierProduct = _mapper.Map<SupplierProduct>(dto);

            await _unitOfWork.SupplierProductRepository.AddAsync(supplierProduct);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SupplierProductDto>(supplierProduct);
        }

        public async Task RemoveProductFromSupplierAsync(int supplierId, int productId)
        {
            var supplierProduct = await _unitOfWork.SupplierProductRepository.GetBySupplierAndProductAsync(supplierId, productId);
            if (supplierProduct != null)
            {
                supplierProduct.IsDeleted = true;
                supplierProduct.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.SupplierProductRepository.Update(supplierProduct);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> SupplierProductExistsAsync(int supplierId, int productId)
        {
            return await _unitOfWork.SupplierProductRepository.AnyAsync(
                sp => sp.SupplierId == supplierId && sp.ProductId == productId && !sp.IsDeleted);
        }

        private SupplierProductDto MapToViewDto(SupplierProduct supplierProduct) => new()
        {
            Id = supplierProduct.Id,
            SupplierId = supplierProduct.SupplierId,
            ProductId = supplierProduct.ProductId,
            CreatedAt = supplierProduct.CreatedAt,
            UpdatedAt = supplierProduct.UpdatedAt,
            IsDeleted = supplierProduct.IsDeleted
        };
}