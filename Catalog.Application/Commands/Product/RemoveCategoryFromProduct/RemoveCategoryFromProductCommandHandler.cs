using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;
using MediatR;

namespace Catalog.Application.Commands.Product.RemoveCategoryFromProduct;

public class RemoveCategoryFromProductCommandHandler : ICommandHandler<RemoveCategoryFromProductCommand>
{
    private readonly IProductService _productService;

    public RemoveCategoryFromProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Unit> Handle(RemoveCategoryFromProductCommand request, CancellationToken cancellationToken)
    {
        await _productService.RemoveCategoryFromProductAsync(request.ProductId, request.CategoryId, cancellationToken);
        return Unit.Value;
    }
}