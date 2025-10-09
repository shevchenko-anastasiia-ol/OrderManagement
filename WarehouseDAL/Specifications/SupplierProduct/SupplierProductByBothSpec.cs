using Ardalis.Specification;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Specifications;

public class SupplierProductByBothSpec : Specification<SupplierProduct>, ISingleResultSpecification<SupplierProduct>
{
    public SupplierProductByBothSpec(int supplierId, int productId)
    {
        Query.Where(sp => sp.SupplierId == supplierId && sp.ProductId == productId);
    }
}