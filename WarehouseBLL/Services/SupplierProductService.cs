using AutoMapper;
using WarehouseBLL.DTOs.SupplierProduct;
using WarehouseBLL.Exceptions; 
using WarehouseBLL.Services.Interfaces;
using WarehouseBLL.Specifications;
using WarehouseBLL.Specifications.Supplier;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services;

public class SupplierProductService : ISupplierProductService
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
        if (supplierProduct == null)
            throw new NotFoundException($"SupplierProduct with ID {id} not found.");

        return _mapper.Map<SupplierProductDto>(supplierProduct);
    }

    public async Task<IEnumerable<SupplierProductDto>> GetSupplierProductsBySupplierAsync(int supplierId)
    {
        var spec = new SupplierProductBySupplierSpec(supplierId);
        var supplierProducts = await _unitOfWork.SupplierProductRepository.ListAsync(spec);

        if (!supplierProducts.Any())
            throw new NotFoundException($"No products found for Supplier with ID {supplierId}.");

        return _mapper.Map<IEnumerable<SupplierProductDto>>(supplierProducts);
    }

    public async Task<IEnumerable<SupplierProductDto>> GetSupplierProductsByProductAsync(int productId)
    {
        var spec = new SupplierProductByProductSpec(productId);
        var supplierProducts = await _unitOfWork.SupplierProductRepository.ListAsync(spec);

        if (!supplierProducts.Any())
            throw new NotFoundException($"No suppliers found for Product with ID {productId}.");

        return _mapper.Map<IEnumerable<SupplierProductDto>>(supplierProducts);
    }

    public async Task<SupplierProductDto?> GetSupplierProductAsync(int supplierId, int productId)
    {
        var spec = new SupplierProductByBothSpec(supplierId, productId);
        var supplierProduct = await _unitOfWork.SupplierProductRepository.SingleOrDefaultAsync(spec);
        
        if (supplierProduct == null)
            throw new NotFoundException($"Supplier {supplierId} does not supply Product {productId}.");

        return _mapper.Map<SupplierProductDto>(supplierProduct);
    }

    public async Task<SupplierProductDto> AddProductToSupplierAsync(SupplierProductCreateDto dto)
    {
        if (await SupplierProductExistsAsync(dto.SupplierId, dto.ProductId))
        {
            throw new ConflictException($"Supplier {dto.SupplierId} already supplies Product {dto.ProductId}.");
        }

        // Перевірка існування постачальника
        var supplierSpec = new SupplierByIdSpec(dto.SupplierId);
        var supplier = await _unitOfWork.SupplierRepository.FirstOrDefaultAsync(supplierSpec);
        if (supplier == null)
            throw new NotFoundException($"Supplier with ID {dto.SupplierId} not found.");

        // Перевірка існування продукту
        var productSpec = new ProductByIdSpecification(dto.ProductId);
        var product = await _unitOfWork.ProductRepository.FirstOrDefaultAsync(productSpec);
        if (product == null)
            throw new NotFoundException($"Product with ID {dto.ProductId} not found.");

        var supplierProduct = _mapper.Map<SupplierProduct>(dto);
        supplierProduct.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.SupplierProductRepository.AddAsync(supplierProduct);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<SupplierProductDto>(supplierProduct);
    }

    public async Task RemoveProductFromSupplierAsync(int supplierId, int productId)
    {
        var spec = new SupplierProductByBothSpec(supplierId, productId);
        var supplierProduct = await _unitOfWork.SupplierProductRepository.SingleOrDefaultAsync(spec);
        
        if (supplierProduct == null)
            throw new NotFoundException($"Supplier {supplierId} does not supply Product {productId}.");

        supplierProduct.IsDeleted = true;
        supplierProduct.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.SupplierProductRepository.Update(supplierProduct);
        await _unitOfWork.SaveChangesAsync();
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
