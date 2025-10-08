using WarehouseBLL.Services.Interfaces;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services;

public class SupplierProductService :  ISupplierProductService
{
    private readonly IUnitOfWork _unitOfWork;

        public SupplierProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SupplierProduct?> GetSupplierProductByIdAsync(int id)
        {
            return await _unitOfWork.SupplierProductRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<SupplierProduct>> GetSupplierProductsBySupplierAsync(int supplierId)
        {
            return await _unitOfWork.SupplierProductRepository.GetBySupplierIdAsync(supplierId);
        }

        public async Task<IEnumerable<SupplierProduct>> GetSupplierProductsByProductAsync(int productId)
        {
            return await _unitOfWork.SupplierProductRepository.GetByProductIdAsync(productId);
        }

        public async Task<SupplierProduct?> GetSupplierProductAsync(int supplierId, int productId)
        {
            return await _unitOfWork.SupplierProductRepository.GetBySupplierAndProductAsync(supplierId, productId);
        }

        public async Task<SupplierProduct> AddProductToSupplierAsync(int supplierId, int productId)
        {
            if (await SupplierProductExistsAsync(supplierId, productId))
            {
                throw new InvalidOperationException(
                    $"Supplier {supplierId} already supplies Product {productId}");
            }

            var supplierProduct = new SupplierProduct
            {
                SupplierId = supplierId,
                ProductId = productId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _unitOfWork.SupplierProductRepository.AddAsync(supplierProduct);
            await _unitOfWork.SaveChangesAsync();

            return supplierProduct;
        }

        public async Task RemoveProductFromSupplierAsync(int supplierId, int productId)
        {
            var supplierProduct = await _unitOfWork.SupplierProductRepository
                .GetBySupplierAndProductAsync(supplierId, productId);

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
}