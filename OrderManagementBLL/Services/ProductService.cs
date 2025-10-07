using AutoMapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.UnitOfWork;
using OrderManagementBLL.DTOs.Product;
using OrderManagementBLL.Exceptions;
using OrderManagementBLL.Services.Interfaces;

namespace OrderManagementBLL.Services
{
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
            if (product == null || product.IsDeleted)
                throw new NotFoundException($"Product with ID {id} was not found.");

            return _mapper.Map<ProductDto>(product);
        }

        // --- Add з можливістю idempotency ---
        public async Task<ProductDto> AddAsync(ProductCreateDto dto, string createdBy)
        {
            if (string.IsNullOrWhiteSpace(dto.ProductName))
                throw new ValidationException("Product name is required.");
            if (dto.Price < 0)
                throw new ValidationException("Product price cannot be negative.");

            // Якщо є IdempotencyToken або унікальний ключ, перевірка на дублювання
            if (!string.IsNullOrEmpty(dto.IdempotencyToken))
            {
                var existingProduct = await _unitOfWork.ProductRepository
                    .GetByIdempotencyTokenAsync(dto.IdempotencyToken);
                if (existingProduct != null)
                    return _mapper.Map<ProductDto>(existingProduct);
            }

            var product = _mapper.Map<Product>(dto);
            product.CreatedAt = DateTime.UtcNow;
            product.CreatedBy = createdBy ?? "system";
            product.IsDeleted = false;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.ProductRepository.AddAsync(product, _unitOfWork.Transaction);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return _mapper.Map<ProductDto>(product);
        }

        // --- Update з оптимістичною конкуренцією ---
        public async Task<ProductDto> UpdateAsync(ProductUpdateDto dto, string updatedBy)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProductId);
            if (product == null || product.IsDeleted)
                throw new NotFoundException($"Product with ID {dto.ProductId} was not found or deleted.");

            if (dto.RowVer != null && !product.RowVer.SequenceEqual(dto.RowVer))
                throw new BusinessConflictException("The product has been modified by another user.");

            _mapper.Map(dto, product);
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = updatedBy ?? "system";

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.ProductRepository.UpdateAsync(product, _unitOfWork.Transaction);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return _mapper.Map<ProductDto>(product);
        }

        // --- Delete з оптимістичною конкуренцією ---
        public async Task DeleteAsync(long id, byte[] rowVer = null, string updatedBy = null)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null || product.IsDeleted)
                throw new NotFoundException("Product is already deleted or does not exist.");

            if (rowVer != null && !product.RowVer.SequenceEqual(rowVer))
                throw new BusinessConflictException("The product has been modified by another user.");

            product.IsDeleted = true;
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = updatedBy ?? "system";

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.ProductRepository.UpdateAsync(product, _unitOfWork.Transaction);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
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
}
