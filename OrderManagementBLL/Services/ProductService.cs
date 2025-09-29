using AutoMapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.UnitOfWork;
using OrderManagementBLL.DTOs.Product;
using OrderManagementBLL.Exceptions;
using OrderManagementBLL.Services.Interfaces;

namespace OrderManagementBLL.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // --- CRUD ---
    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _unitOfWork.ProductRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> GetByIdAsync(long id)
    {
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
        if (product == null)
            throw new NotFoundException($"Product with ID {id} was not found.");

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> AddAsync(ProductCreateDto dto, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(dto.ProductName))
            throw new ValidationException("Product name is required.");
        if (dto.Price < 0)
            throw new ValidationException("Product price cannot be negative.");

        var product = _mapper.Map<Product>(dto);
        product.CreatedAt = DateTime.UtcNow;
        product.CreatedBy = createdBy;

        await _unitOfWork.ProductRepository.AddAsync(product);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> UpdateAsync(ProductUpdateDto dto, string updatedBy)
    {
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
            throw new NotFoundException($"Product with ID {dto.ProductId} was not found.");

        _mapper.Map(dto, product);
        product.UpdatedAt = DateTime.UtcNow;
        product.UpdatedBy = updatedBy;

        await _unitOfWork.ProductRepository.UpdateAsync(product);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task DeleteAsync(long id)
    {
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
        if (product == null)
            throw new NotFoundException($"Product with ID {id} was not found.");

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.ProductRepository.UpdateAsync(product);
        await _unitOfWork.SaveAsync();
    }

    // --- Спеціальні методи ---
    public async Task<IEnumerable<ProductDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        if (minPrice < 0 || maxPrice < 0)
            throw new ValidationException("Price range cannot be negative.");
        if (minPrice > maxPrice)
            throw new ValidationException("Min price cannot be greater than max price.");

        var products = await _unitOfWork.ProductRepository.GetProductsByPriceRangeAsync(minPrice, maxPrice);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsInStockAsync()
    {
        var products = await _unitOfWork.ProductRepository.GetProductsInStockAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> FindProductsByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Product name cannot be empty.");

        var products = await _unitOfWork.ProductRepository.FindProductsByNameAsync(name);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<int> CountProductsInStockAsync()
    {
        return await _unitOfWork.ProductRepository.CountProductsInStockAsync();
    }

    public async Task<List<string>> GetDistinctProductNamesAsync()
    {
        return await _unitOfWork.ProductRepository.GetDistinctProductNamesAsync();
    }

    public async Task<ProductDto> GetProductWithOrderItemsAsync(long productId)
    {
        var product = await _unitOfWork.ProductRepository.GetProductWithOrderItemsAsync(productId);
        if (product == null)
            throw new NotFoundException($"Product with ID {productId} was not found.");

        return _mapper.Map<ProductDto>(product);
    }
}
