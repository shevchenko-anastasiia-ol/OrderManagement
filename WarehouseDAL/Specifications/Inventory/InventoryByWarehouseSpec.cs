using Ardalis.Specification;

namespace WarehouseBLL.Specifications.Inventory;

public class InventoryByWarehouseSpec : Specification<WarehouseDomain.Entities.Inventory>
{
    public InventoryByWarehouseSpec(int warehouseId)
    {
        Query.Where(i => i.WarehouseId == warehouseId)
            .Include(i => i.Product)
            .OrderBy(i => i.Product.Name);
    }
}