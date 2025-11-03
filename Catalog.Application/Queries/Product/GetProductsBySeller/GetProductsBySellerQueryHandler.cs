using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Product.GetProductsBySeller;

public class GetProductsBySellerQueryHandler : IQueryHandler<GetProductsBySellerQuery, IEnumerable<Domain.Entities.Product>>
{
    private readonly IProductService _productService;

    public GetProductsBySellerQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IEnumerable<Domain.Entities.Product>> Handle(GetProductsBySellerQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetBySellerAsync(request.SellerId, cancellationToken);
    }
}