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

        public async Task<Supplier?> GetSupplierByIdAsync(int id)
        {
            return await _unitOfWork.SupplierRepository.GetByIdAsync(id);
        }

        public async Task<Supplier?> GetSupplierWithProductsAsync(int id)
        {
            return await _unitOfWork.SupplierRepository.GetByIdWithProductsAsync(id);
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            return await _unitOfWork.SupplierRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Supplier>> GetSuppliersByCountryAsync(string country)
        {
            return await _unitOfWork.SupplierRepository.GetSuppliersByCountryAsync(country);
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersWithProductsAsync()
        {
            return await _unitOfWork.SupplierRepository.GetAllWithProductsAsync();
        }

        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            supplier.CreatedAt = DateTime.UtcNow;
            supplier.IsDeleted = false;
            
            await _unitOfWork.SupplierRepository.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();
            
            return supplier;
        }

        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            supplier.UpdatedAt = DateTime.UtcNow;
            
            _unitOfWork.SupplierRepository.Update(supplier);
            await _unitOfWork.SaveChangesAsync();
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
}