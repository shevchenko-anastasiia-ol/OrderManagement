using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Seller.SellerHasProducts;

public class SellerHasProductsQueryHandler : IQueryHandler<SellerHasProductsQuery, bool>
{
    private readonly ISellerService _sellerService;

    public SellerHasProductsQueryHandler(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    public async Task<bool> Handle(SellerHasProductsQuery request, CancellationToken cancellationToken)
    {
        return await _sellerService.HasProductsAsync(request.SellerId, cancellationToken);
    }
}