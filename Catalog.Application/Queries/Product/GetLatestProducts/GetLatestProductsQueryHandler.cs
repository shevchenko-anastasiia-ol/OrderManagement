using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Product.GetLatestProducts;

public class GetLatestProductsQueryHandler : IQueryHandler<GetLatestProductsQuery, IEnumerable<Domain.Entities.Product>>
{
    private readonly IProductService _productService;

    public GetLatestProductsQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IEnumerable<Domain.Entities.Product>> Handle(GetLatestProductsQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetLatestProductsAsync(request.Count, cancellationToken);
    }
}