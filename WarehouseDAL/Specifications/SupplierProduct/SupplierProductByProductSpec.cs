using Ardalis.Specification;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Specifications;

public class SupplierProductByProductSpec : Specification<SupplierProduct>
{
    public SupplierProductByProductSpec(int productId)
    {
        Query.Where(sp => sp.ProductId == productId)
            .Include(sp => sp.Supplier)
            .OrderBy(sp => sp.Supplier.Name);
    }
}