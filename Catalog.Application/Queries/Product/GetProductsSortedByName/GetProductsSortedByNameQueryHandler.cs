using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Product.GetProductsSortedByName;

public class GetProductsSortedByNameQueryHandler : IQueryHandler<GetProductsSortedByNameQuery, IEnumerable<Domain.Entities.Product>>
{
    private readonly IProductService _productService;

    public GetProductsSortedByNameQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IEnumerable<Domain.Entities.Product>> Handle(GetProductsSortedByNameQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetProductsSortedByNameAsync(request.Ascending, request.Limit, cancellationToken);
    }
}