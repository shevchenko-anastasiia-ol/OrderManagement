using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Product.GetProductsByPriceRange;

public class GetProductsByPriceRangeQueryHandler : IQueryHandler<GetProductsByPriceRangeQuery, IEnumerable<Domain.Entities.Product>>
{
    private readonly IProductService _productService;

    public GetProductsByPriceRangeQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IEnumerable<Domain.Entities.Product>> Handle(GetProductsByPriceRangeQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetByPriceRangeAsync(request.MinPrice, request.MaxPrice, request.Currency, cancellationToken);
    }
}