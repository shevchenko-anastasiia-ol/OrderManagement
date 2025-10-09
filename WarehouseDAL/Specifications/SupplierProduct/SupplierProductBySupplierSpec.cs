using Ardalis.Specification;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Specifications;

public class SupplierProductBySupplierSpec : Specification<SupplierProduct>
{
    public SupplierProductBySupplierSpec(int supplierId)
    {
        Query.Where(sp => sp.SupplierId == supplierId)
            .Include(sp => sp.Product)
            .OrderBy(sp => sp.Product.Name);
    }
}