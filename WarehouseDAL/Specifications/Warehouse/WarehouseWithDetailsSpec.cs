using Ardalis.Specification;

namespace WarehouseBLL.Specifications.Warehouse;

public class WarehousesWithDetailsSpec : Specification<WarehouseDomain.Entities.Warehouse>
{
    public WarehousesWithDetailsSpec()
    {
        Query.Where(w => !w.IsDeleted)
            .Include(w => w.Details)
            .OrderBy(w => w.Name);
    }
}