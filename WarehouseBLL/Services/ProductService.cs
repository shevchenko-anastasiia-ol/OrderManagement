using AutoMapper;
using WarehouseBLL.DTOs.Inventory;
using WarehouseBLL.DTOs.Product;
using WarehouseBLL.DTOs.Supplier;
using WarehouseBLL.Helpers;
using WarehouseBLL.Services.Interfaces;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;
using WarehouseBLL.Exceptions;
using WarehouseBLL.Specifications;

namespace WarehouseBLL.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var spec = new ProductByIdSpecification(id);
        var product = await _unitOfWork.ProductRepository.SingleOrDefaultAsync(spec);
        
        if (product == null)
            throw new NotFoundException($"Product with ID {id} not found.");

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto?> GetProductBySkuAsync(string sku)
    {
        var spec = new ProductBySkuSpecification(sku);
        var product = _unitOfWork.ProductRepository.SingleOrDefaultAsync(spec);
        
        if (product == null)
            throw new NotFoundException($"Product with SKU '{sku}' not found.");

        return _mapper.Map<ProductDto>(product);
        
    }

    public async Task<ProductWithInventoryDto?> GetProductWithInventoryAsync(int id)
    {
        var spec = new ProductByIdSpecification(id, includeInventories: true);
        var product = await _unitOfWork.ProductRepository.SingleOrDefaultAsync(spec);
        
        if (product == null)
            throw new NotFoundException($"Product with ID {id} not found.");

        return _mapper.Map<ProductWithInventoryDto>(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var queryParams = new ProductQueryParams { Page = 1, PageSize = int.MaxValue };
        var spec = new ProductSpecification(queryParams);
        var products = await _unitOfWork.ProductRepository.ListAsync(spec);
        
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        var queryParams = new ProductQueryParams 
        { 
            MinPrice = minPrice, 
            MaxPrice = maxPrice,
            Page = 1,
            PageSize = int.MaxValue
        };
        var spec = new ProductSpecification(queryParams);
        var products = await _unitOfWork.ProductRepository.ListAsync(spec);
        
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductWithSuppliersDto?> GetProductWithSuppliersAsync(int id)
    {
        var spec = new ProductByIdSpecification(id, includeSuppliers: true);
        var product = await _unitOfWork.ProductRepository.SingleOrDefaultAsync(spec);
        
        if (product == null)
            throw new NotFoundException($"Product with ID {id} not found.");

        return _mapper.Map<ProductWithSuppliersDto>(product);
    }
    
    public async Task<IEnumerable<ProductWithSuppliersDto>> GetProductsWithSuppliersAsync()
    {
        var queryParams = new ProductQueryParams { Page = 1, PageSize = int.MaxValue };
        var spec = new ProductSpecification(queryParams);
        var products = await _unitOfWork.ProductRepository.ListAsync(spec);
        
        return _mapper.Map<IEnumerable<ProductWithSuppliersDto>>(products);
    }
    
    public async Task<List<ProductDto>> GetLowStockProductsAsync(int threshold = 10)
    {
        var spec = new LowStockProductSpecification(threshold);
        var products = await _unitOfWork.ProductRepository.ListAsync(spec);
        
        return _mapper.Map<List<ProductDto>>(products);
    }

    public async Task<List<ProductDto>> GetRecentProductsAsync(int count = 10)
    {
        var spec = new RecentProductsSpec(count);
        var products = await _unitOfWork.ProductRepository.ListAsync(spec);
        
        return _mapper.Map<List<ProductDto>>(products);
    }

    public async Task<ProductDto> CreateProductAsync(ProductCreateDto dto)
    {
        if (await SkuExistsAsync(dto.SKU))
        {
            throw new ConflictException($"Product with SKU '{dto.SKU}' already exists.");
        }

        var product = _mapper.Map<Product>(dto);

        await _unitOfWork.ProductRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> UpdateProductAsync(ProductUpdateDto dto)
    {
        var spec = new ProductByIdSpecification(dto.Id);
        var product = await _unitOfWork.ProductRepository.SingleOrDefaultAsync(spec);
        
        if (product == null)
            throw new NotFoundException($"Product with ID {dto.Id} not found.");

        // Перевірка SKU при зміні
        if (product.SKU != dto.SKU && await SkuExistsAsync(dto.SKU))
        {
            throw new ConflictException($"Product with SKU '{dto.SKU}' already exists.");
        }

        _mapper.Map(dto, product);
        product.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.ProductRepository.Update(product);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task DeleteProductAsync(int id)
    {
        var spec = new ProductByIdSpecification(id);
        var product = await _unitOfWork.ProductRepository.SingleOrDefaultAsync(spec);
        
        if (product == null)
            throw new NotFoundException($"Product with ID {id} not found.");

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.ProductRepository.Update(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ProductExistsAsync(int id)
    {
        var spec = new ProductByIdSpecification(id);
        return await _unitOfWork.ProductRepository.AnyAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<bool> SkuExistsAsync(string sku)
    {
        var spec = new ProductBySkuSpecification(sku);
        return await _unitOfWork.ProductRepository.AnyAsync(p => p.SKU == sku && !p.IsDeleted);
    }

    private ProductDto MapToViewDto(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        SKU = product.SKU,
        Price = product.Price,
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt,
        IsDeleted = product.IsDeleted
    };

    private ProductWithInventoryDto MapToWithInventoryDto(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        SKU = product.SKU,
        Price = product.Price,
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt,
        IsDeleted = product.IsDeleted,
        Inventories = product.Inventories.Select(i => new InventoryDto
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

    private ProductWithSuppliersDto MapToWithSuppliersDto(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        SKU = product.SKU,
        Price = product.Price,
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt,
        IsDeleted = product.IsDeleted,
        Suppliers = product.SupplierProducts.Select(sp => new SupplierDto
        {
            Id = sp.Supplier.Id,
            Name = sp.Supplier.Name,
            Country = sp.Supplier.Country,
            ContactInfo = sp.Supplier.ContactInfo,
            CreatedAt = sp.Supplier.CreatedAt,
            UpdatedAt = sp.Supplier.UpdatedAt,
            IsDeleted = sp.Supplier.IsDeleted
        }).ToList()
    };

    public async Task<PagedResult<ProductDto>> GetProductsPagedAsync(ProductQueryParams queryParams)
    {
        var spec = new ProductSpecification(queryParams);
        var countSpec = new ProductCountSpecification(queryParams);

        var products = await _unitOfWork.ProductRepository.ListAsync(spec);
        var totalCount = await _unitOfWork.ProductRepository.CountAsync(countSpec);

        var items = _mapper.Map<IEnumerable<ProductDto>>(products);

        return new PagedResult<ProductDto>(items, totalCount, queryParams.Page, queryParams.PageSize);
    }

}
