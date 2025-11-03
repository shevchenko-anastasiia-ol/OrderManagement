using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Seller.GetSellerProductCount;

public class GetSellerProductCountQuery : IQuery<long>
{
    public string SellerId { get; init; } = default!;
}