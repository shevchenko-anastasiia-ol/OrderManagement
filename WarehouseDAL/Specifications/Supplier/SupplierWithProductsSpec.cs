using Ardalis.Specification;

namespace WarehouseBLL.Specifications.Supplier;

public class SuppliersWithProductsSpec : Specification<WarehouseDomain.Entities.Supplier>
{
    public SuppliersWithProductsSpec()
    {
        Query.Where(s => !s.IsDeleted)
            .Include(s => s.SupplierProducts)
            .ThenInclude(sp => sp.Product)
            .OrderBy(s => s.Name);
    }
}