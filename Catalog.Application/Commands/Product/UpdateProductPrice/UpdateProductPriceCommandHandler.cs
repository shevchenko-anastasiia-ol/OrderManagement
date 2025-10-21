using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Commands.Product.UpdateProductPrice;

public class UpdateProductPriceCommandHandler : ICommandHandler<UpdateProductPriceCommand, Domain.Entities.Product>
{
    private readonly IProductService _productService;
    public UpdateProductPriceCommandHandler(IProductService productService)
    {
        _productService = productService;
    }
    
    public async Task<Domain.Entities.Product> Handle(UpdateProductPriceCommand request, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(request.ProductId, cancellationToken);

        await _productService.UpdatePriceAsync(product, cancellationToken);
        return product;
    }
}