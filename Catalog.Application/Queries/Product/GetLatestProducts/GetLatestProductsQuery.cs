using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Product.GetLatestProducts;

public class GetLatestProductsQuery : IQuery<IEnumerable<Domain.Entities.Product>>
{
    public int Count { get; init; } = 10;
}