using Ardalis.Specification;

namespace WarehouseBLL.Specifications.Inventory;

public class InventoryByWarehouseAndProductSpec : Specification<WarehouseDomain.Entities.Inventory>, ISingleResultSpecification<WarehouseDomain.Entities.Inventory>
{
    public InventoryByWarehouseAndProductSpec(int warehouseId, int productId)
    {
        Query.Where(i => i.WarehouseId == warehouseId && i.ProductId == productId)
            .Include(i => i.Warehouse)
            .Include(i => i.Product);
    }
}