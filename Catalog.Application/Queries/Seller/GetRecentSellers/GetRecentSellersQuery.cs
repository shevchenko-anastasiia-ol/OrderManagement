using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Seller.GetRecentSellers;

public class GetRecentSellersQuery : IQuery<IEnumerable<Domain.Entities.Seller>>
{
    public int Count { get; init; } = 10;
}
