using Ardalis.Specification;

namespace WarehouseBLL.Specifications.Warehouse;

public class WarehouseByIdSpec : Specification<WarehouseDomain.Entities.Warehouse>, ISingleResultSpecification<WarehouseDomain.Entities.Warehouse>
{
    public WarehouseByIdSpec(int id, bool includeDetails = false, bool includeInventory = false)
    {
        Query.Where(w => w.Id == id && !w.IsDeleted);

        if (includeDetails)
            Query.Include(w => w.Details);

        if (includeInventory)
            Query.Include(w => w.Inventories)
                .ThenInclude(i => i.Product);
    }
}