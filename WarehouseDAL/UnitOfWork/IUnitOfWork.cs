using WarehouseDAL.Repositories.Interfaces;

namespace WarehouseDAL.UnitOfWork;

public interface IUnitOfWork
{
    IInventoryRepository  InventoryRepository { get; }
    IProductRepository ProductRepository { get; }
    IWarehouseRepository WarehouseRepository { get; }
    ISupplierRepository SupplierRepository { get; }
    ISupplierProductRepository SupplierProductRepository { get; }
    IWarehouseDetailRepository WarehouseDetailRepository { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}