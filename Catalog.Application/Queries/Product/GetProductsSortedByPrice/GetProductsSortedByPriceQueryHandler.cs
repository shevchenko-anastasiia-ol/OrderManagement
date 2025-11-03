using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Product.GetProductsSortedByPrice;

public class GetProductsSortedByPriceQueryHandler : IQueryHandler<GetProductsSortedByPriceQuery, IEnumerable<Domain.Entities.Product>>
{
    private readonly IProductService _productService;

    public GetProductsSortedByPriceQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IEnumerable<Domain.Entities.Product>> Handle(GetProductsSortedByPriceQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetProductsSortedByPriceAsync(request.Ascending, request.Limit, cancellationToken);
    }
}