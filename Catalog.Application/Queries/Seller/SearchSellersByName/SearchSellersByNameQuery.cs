using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Seller.SearchSellersByName;

public class SearchSellersByNameQuery : IQuery<IEnumerable<Domain.Entities.Seller>>
{
    public string Name { get; init; } = default!;
}