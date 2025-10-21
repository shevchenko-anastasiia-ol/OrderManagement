using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Commands.Product.AddProductImages;

public class AddProductImagesCommandHandler : ICommandHandler<AddProductImagesCommand, Domain.Entities.Product>
{
    private readonly IProductService _productService;
    public AddProductImagesCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Domain.Entities.Product> Handle(AddProductImagesCommand request, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(request.ProductId, cancellationToken);

        if (product.ProductDetail == null)
        {
            product.SetProductDetail(new ProductDetail("Product description"));
        }

        foreach (var imageUrl in request.ImageUrls)
        {
            product.ProductDetail!.AddImage(imageUrl);
        }

        await _productService.UpdateAsync(product, cancellationToken);
        return product;
    }
}