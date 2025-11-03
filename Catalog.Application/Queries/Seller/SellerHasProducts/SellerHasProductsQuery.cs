using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Seller.SellerHasProducts;

public class SellerHasProductsQuery : IQuery<bool>
{
    public string SellerId { get; init; } = default!;
}