using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;
using MediatR;

namespace Catalog.Application.Commands.Product.DeleteProduct;

public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand>
{
    private readonly IProductService _productService;

    public DeleteProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Unit>  Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        await _productService.DeleteAsync(
            request.ProductId,
            request.DeleteReviews, 
            cancellationToken);
        return Unit.Value;
    }
}