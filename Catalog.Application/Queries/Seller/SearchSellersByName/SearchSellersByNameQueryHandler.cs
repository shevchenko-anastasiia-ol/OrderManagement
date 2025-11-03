using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Seller.SearchSellersByName;

public class SearchSellersByNameQueryHandler : IQueryHandler<SearchSellersByNameQuery, IEnumerable<Domain.Entities.Seller>>
{
    private readonly ISellerService _sellerService;

    public SearchSellersByNameQueryHandler(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    public async Task<IEnumerable<Domain.Entities.Seller>> Handle(SearchSellersByNameQuery request, CancellationToken cancellationToken)
    {
        return await _sellerService.SearchByNameAsync(request.Name, cancellationToken);
    }
}