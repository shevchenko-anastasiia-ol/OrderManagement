using Catalog.Application.Interfaces.Queries;
using Catalog.Application.Queries.Review.GetReviews;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Product.GetProductById;

public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, Domain.Entities.Product>
{
    private readonly IProductService _productService;
    public  GetProductByIdQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Domain.Entities.Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetByIdAsync(request.ProductId, cancellationToken);
    }
}