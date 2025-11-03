using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Seller.GetSellerByPhone;

public class GetSellerByPhoneQueryHandler : IQueryHandler<GetSellerByPhoneQuery, Domain.Entities.Seller?>
{
    private readonly ISellerService _sellerService;

    public GetSellerByPhoneQueryHandler(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    public async Task<Domain.Entities.Seller?> Handle(GetSellerByPhoneQuery request, CancellationToken cancellationToken)
    {
        return await _sellerService.GetByPhoneAsync(request.Phone, cancellationToken);
    }
}