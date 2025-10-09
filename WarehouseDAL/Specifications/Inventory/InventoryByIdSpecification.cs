using Ardalis.Specification;

namespace WarehouseBLL.Specifications.Inventory;

public class InventoryByIdSpecification : Specification<WarehouseDomain.Entities.Inventory>, ISingleResultSpecification<WarehouseDomain.Entities.Inventory>
{
    public InventoryByIdSpecification(int id)
    {
        Query.Where(i => i.Id == id)
            .Include(i => i.Warehouse)
            .Include(i => i.Product);
    }
}