using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Commands.Product.CreateProduct;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Domain.Entities.Product>
{
    private readonly IProductService _productService;

    public CreateProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Domain.Entities.Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Domain.Entities.Product(request.Name, request.Price, request.SellerId, request.UserId);

        // Додавання категорій
        foreach (var categoryId in request.CategoryIds)
        {
            product.AddCategory(categoryId);
        }
        
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

        await _productService.CreateAsync(product, cancellationToken);
        return product;
    }
}