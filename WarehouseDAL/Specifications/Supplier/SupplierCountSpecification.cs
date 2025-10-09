using Ardalis.Specification;
using WarehouseBLL.Helpers;

namespace WarehouseBLL.Specifications.Supplier;

public class SupplierCountSpecification : Specification<WarehouseDomain.Entities.Supplier>
{
    public SupplierCountSpecification(SupplierQueryParams queryParams)
    {
        Query.Where(s => !s.IsDeleted);

        if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            Query.Where(s => s.Name.Contains(queryParams.SearchTerm) || 
                             s.ContactInfo.Contains(queryParams.SearchTerm));

        if (!string.IsNullOrWhiteSpace(queryParams.Country))
            Query.Where(s => s.Country == queryParams.Country);
    }
}