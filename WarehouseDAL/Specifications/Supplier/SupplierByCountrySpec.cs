using Ardalis.Specification;

namespace WarehouseBLL.Specifications.Supplier;

public class SuppliersByCountrySpec : Specification<WarehouseDomain.Entities.Supplier>
{
    public SuppliersByCountrySpec(string country)
    {
        Query.Where(s => s.Country == country && !s.IsDeleted)
            .OrderBy(s => s.Name);
    }
}