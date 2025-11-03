using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Product.GetProductCountBySeller;

public class GetProductCountBySellerQueryHandler : IQueryHandler<GetProductCountBySellerQuery, Dictionary<string, long>>
{
    private readonly IProductService _productService;

    public GetProductCountBySellerQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Dictionary<string, long>> Handle(GetProductCountBySellerQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetProductCountBySellerAsync(cancellationToken);
    }
}