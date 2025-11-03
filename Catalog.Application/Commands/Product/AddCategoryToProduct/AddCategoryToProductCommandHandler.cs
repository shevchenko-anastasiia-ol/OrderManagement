using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;
using MediatR;

namespace Catalog.Application.Commands.Product.AddCategoryToProduct;

public class AddCategoryToProductCommandHandler : ICommandHandler<AddCategoryToProductCommand>
{
    private readonly IProductService _productService;

    public AddCategoryToProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Unit> Handle(AddCategoryToProductCommand request, CancellationToken cancellationToken)
    {
        await _productService.AddCategoryToProductAsync(request.ProductId, request.CategoryId, cancellationToken);
        return Unit.Value;
    }
}