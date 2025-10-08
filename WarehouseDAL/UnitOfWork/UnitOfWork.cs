using Microsoft.EntityFrameworkCore;
using WarehouseDAL.Data;
using WarehouseDAL.Repositories.Interfaces;

namespace WarehouseDAL.UnitOfWork;

public class UnitOfWork :  IUnitOfWork
{
    public readonly WarehouseDbContext _context;
    
    public IInventoryRepository  InventoryRepository { get; }
    public IProductRepository ProductRepository { get; }
    public ISupplierRepository SupplierRepository { get; }
    public IWarehouseRepository WarehouseRepository { get; }
    public ISupplierProductRepository SupplierProductRepository { get; }
    public IWarehouseDetailRepository WarehouseDetailRepository { get; }

    public UnitOfWork(WarehouseDbContext context,
        IInventoryRepository inventoryRepository,
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        IWarehouseRepository warehouseRepository,
        ISupplierProductRepository supplierProductRepository,
        IWarehouseDetailRepository warehouseDetailRepository)
    {
        _context = context;
        InventoryRepository = inventoryRepository;
        ProductRepository = productRepository;
        SupplierRepository = supplierRepository;
        WarehouseRepository = warehouseRepository;
        SupplierProductRepository = supplierProductRepository;
        WarehouseDetailRepository = warehouseDetailRepository;
    }
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}