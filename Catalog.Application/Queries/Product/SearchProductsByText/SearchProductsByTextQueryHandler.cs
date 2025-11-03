using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Product.SearchProductsByText;

public class SearchProductsByTextQueryHandler : IQueryHandler<SearchProductsByTextQuery, IEnumerable<Domain.Entities.Product>>
{
    private readonly IProductService _productService;

    public SearchProductsByTextQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IEnumerable<Domain.Entities.Product>> Handle(SearchProductsByTextQuery request, CancellationToken cancellationToken)
    {
        return await _productService.SearchByTextAsync(request.SearchTerm, request.Limit, cancellationToken);
    }
}