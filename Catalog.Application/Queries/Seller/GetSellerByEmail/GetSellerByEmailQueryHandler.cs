using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Seller.GetSellerByEmail;

public class GetSellerByEmailQueryHandler : IQueryHandler<GetSellerByEmailQuery, Domain.Entities.Seller?>
{
    private readonly ISellerService _sellerService;

    public GetSellerByEmailQueryHandler(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    public async Task<Domain.Entities.Seller?> Handle(GetSellerByEmailQuery request, CancellationToken cancellationToken)
    {
        return await _sellerService.GetByEmailAsync(request.Email, cancellationToken);
    }
}