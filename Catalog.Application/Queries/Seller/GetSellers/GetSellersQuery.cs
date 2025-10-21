using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Entities.Parameters;

namespace Catalog.Application.Queries.Seller.GetSellers;

public class GetSellersQuery : IQuery<PagedList<Domain.Entities.Seller>>
{
    public SellerParameters Parameters { get; init; } = default!;
}