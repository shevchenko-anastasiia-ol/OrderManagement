using System.Text;
using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Entities.Parameters;

namespace Catalog.Application.Queries.Product.GetProducts;

public class GetProductsQuery : IQuery<PagedList<Domain.Entities.Product>>
{
    public ProductParameters Parameters { get; init; } = default!;
}