using Ardalis.Specification;

namespace WarehouseBLL.Specifications.Inventory;

public class InventoryByProductSpec : Specification<WarehouseDomain.Entities.Inventory>
{
    public InventoryByProductSpec(int productId)
    {
        Query.Where(i => i.ProductId == productId)
            .Include(i => i.Warehouse)
            .OrderBy(i => i.Warehouse.Name);
    }
}