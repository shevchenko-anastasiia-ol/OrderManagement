using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Seller.GetSellers;

public class GetSellersQueryHandler : IQueryHandler<GetSellersQuery, PagedList<Domain.Entities.Seller>>
{
    private readonly ISellerService _sellerService;
    public GetSellersQueryHandler(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    public async Task<PagedList<Domain.Entities.Seller>> Handle(GetSellersQuery request,
        CancellationToken cancellationToken)
    {
        return await _sellerService.GetAllAsync(request.Parameters, cancellationToken);
    }
}