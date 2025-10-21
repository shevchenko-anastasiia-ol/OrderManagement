using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Seller.GetSellerById;

public class GetSellerByIdQueryHandler : IQueryHandler<GetSellerByIdQuery, Domain.Entities.Seller>
{
    private readonly ISellerService _sellerService;

    public GetSellerByIdQueryHandler(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    public async Task<Domain.Entities.Seller> Handle(GetSellerByIdQuery request, CancellationToken cancellationToken)
    {
        return await _sellerService.GetByIdAsync(request.SellerId, cancellationToken);
    }
}