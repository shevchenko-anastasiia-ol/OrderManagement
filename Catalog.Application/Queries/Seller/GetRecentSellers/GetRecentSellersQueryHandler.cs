using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Seller.GetRecentSellers;

public class GetRecentSellersQueryHandler : IQueryHandler<GetRecentSellersQuery, IEnumerable<Domain.Entities.Seller>>
{
    private readonly ISellerService _sellerService;

    public GetRecentSellersQueryHandler(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    public async Task<IEnumerable<Domain.Entities.Seller>> Handle(GetRecentSellersQuery request, CancellationToken cancellationToken)
    {
        return await _sellerService.GetRecentSellersAsync(request.Count, cancellationToken);
    }
}