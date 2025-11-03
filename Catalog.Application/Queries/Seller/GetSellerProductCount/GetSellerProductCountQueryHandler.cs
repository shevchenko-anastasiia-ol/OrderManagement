using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Seller.GetSellerProductCount;

public class GetSellerProductCountQueryHandler : IQueryHandler<GetSellerProductCountQuery, long>
{
    private readonly ISellerService _sellerService;

    public GetSellerProductCountQueryHandler(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    public async Task<long> Handle(GetSellerProductCountQuery request, CancellationToken cancellationToken)
    {
        return await _sellerService.GetProductCountAsync(request.SellerId, cancellationToken);
    }
}