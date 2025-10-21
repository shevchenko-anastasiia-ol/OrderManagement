using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Commands.Product.UpdateProduct;

public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, Domain.Entities.Product>
{
private readonly IProductService _productService;

public UpdateProductCommandHandler(IProductService productService)
{
    _productService = productService;
}

public async Task<Domain.Entities.Product> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
{
    var product = await _productService.GetByIdAsync(request.ProductId, cancellationToken);

    // Оновлення базових полів
    product.Update(request.Name, request.Price, request.UserId);

    // Оновлення категорій
    if (request.CategoryIds != null)
    {
        foreach (var oldCategoryId in product.Categories.ToList())
        {
            product.RemoveCategory(oldCategoryId);
        }

        foreach (var categoryId in request.CategoryIds)
        {
            product.AddCategory(categoryId);
        }
    }

    // Оновлення деталей
    if (!string.IsNullOrEmpty(request.Description))
    {
        var specs = request.Specifications != null
            ? new Dictionary<string, string>(request.Specifications)
            : null;

        var detail = new ProductDetail(request.Description, specs);

        if (request.ImageUrls != null)
        {
            foreach (var imageUrl in request.ImageUrls)
            {
                detail.AddImage(imageUrl);
            }
        }

        product.SetProductDetail(detail);
    }

    await _productService.UpdateAsync(product, cancellationToken);
    return product;
}
}