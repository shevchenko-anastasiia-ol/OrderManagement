using AutoMapper;
using WarehouseBLL.DTOs.Inventory;
using WarehouseBLL.DTOs.Product;
using WarehouseBLL.DTOs.Supplier;
using WarehouseBLL.Helpers;
using WarehouseBLL.Services.Interfaces;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;

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
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> GetProductBySkuAsync(string sku)
        {
            var product = await _unitOfWork.ProductRepository.GetBySkuAsync(sku);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductWithInventoryDto?> GetProductWithInventoryAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdWithInventoryAsync(id);
            return _mapper.Map<ProductWithInventoryDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var products = await _unitOfWork.ProductRepository.GetProductsByPriceRangeAsync(minPrice, maxPrice);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductWithSuppliersDto>> GetProductsWithSuppliersAsync()
        {
            var products = await _unitOfWork.ProductRepository.GetProductsWithSuppliersAsync();
            return _mapper.Map<IEnumerable<ProductWithSuppliersDto>>(products);
        }

        public async Task<ProductDto> CreateProductAsync(ProductCreateDto dto)
        {
            if (await SkuExistsAsync(dto.SKU))
            {
                throw new InvalidOperationException($"Product with SKU '{dto.SKU}' already exists.");
            }

            var product = _mapper.Map<Product>(dto);

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> UpdateProductAsync(ProductUpdateDto dto)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.Id);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {dto.Id} not found.");

            _mapper.Map(dto, product);

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product != null)
            {
                product.IsDeleted = true;
                product.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.ProductRepository.Update(product);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _unitOfWork.ProductRepository.AnyAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<bool> SkuExistsAsync(string sku)
        {
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
            var query = _unitOfWork.ProductRepository.GetAllAsync().Result.AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(queryParams.SearchTerm) || 
                                         p.SKU.Contains(queryParams.SearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.Sku))
            {
                query = query.Where(p => p.SKU == queryParams.Sku);
            }

            if (queryParams.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= queryParams.MinPrice.Value);
            }

            if (queryParams.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= queryParams.MaxPrice.Value);
            }

            // Sorting
            query = query.ApplySorting(queryParams.SortBy ?? "Name", queryParams.SortDirection);

            // Pagination
            var pagedResult = await query.ToPagedResultAsync(queryParams.Page, queryParams.PageSize);

            return new PagedResult<ProductDto>(
                pagedResult.Items.Select(_mapper.Map<ProductDto>),
                pagedResult.TotalCount,
                pagedResult.Page,
                pagedResult.PageSize
            );
        }
}