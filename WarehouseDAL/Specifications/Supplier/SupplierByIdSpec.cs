using Ardalis.Specification;

namespace WarehouseBLL.Specifications.Supplier;

public class SupplierByIdSpec : Specification<WarehouseDomain.Entities.Supplier>, ISingleResultSpecification<WarehouseDomain.Entities.Supplier>
{
    public SupplierByIdSpec(int id, bool includeProducts = false)
    {
        Query.Where(s => s.Id == id && !s.IsDeleted);

        if (includeProducts)
            Query.Include(s => s.SupplierProducts)
                .ThenInclude(sp => sp.Product);
    }
}