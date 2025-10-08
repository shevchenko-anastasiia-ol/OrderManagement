using WarehouseBLL.Services.Interfaces;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _unitOfWork.ProductRepository.GetByIdAsync(id);
        }

        public async Task<Product?> GetProductBySkuAsync(string sku)
        {
            return await _unitOfWork.ProductRepository.GetBySkuAsync(sku);
        }

        public async Task<Product?> GetProductWithInventoryAsync(int id)
        {
            return await _unitOfWork.ProductRepository.GetByIdWithInventoryAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _unitOfWork.ProductRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _unitOfWork.ProductRepository.GetProductsByPriceRangeAsync(minPrice, maxPrice);
        }

        public async Task<IEnumerable<Product>> GetProductsWithSuppliersAsync()
        {
            return await _unitOfWork.ProductRepository.GetProductsWithSuppliersAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            if (await SkuExistsAsync(product.SKU))
            {
                throw new InvalidOperationException($"Product with SKU '{product.SKU}' already exists.");
            }

            product.CreatedAt = DateTime.UtcNow;
            product.IsDeleted = false;
            
            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            
            return product;
        }

        public async Task UpdateProductAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();
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
}