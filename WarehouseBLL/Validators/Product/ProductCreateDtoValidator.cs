using FluentValidation;
using WarehouseBLL.DTOs.Product;
using WarehouseBLL.Services.Interfaces;

namespace WarehouseBLL.Validators.Product;

public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
{
    private readonly IProductService _productService;

    public ProductCreateDtoValidator(IProductService productService)
    {
        _productService = productService;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters")
            .MinimumLength(3).WithMessage("Product name must be at least 3 characters");

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required")
            .MaximumLength(100).WithMessage("SKU must not exceed 100 characters")
            .Matches("^[A-Za-z0-9-_]+$").WithMessage("SKU can only contain alphanumeric characters, hyphens, and underscores")
            .MustAsync(BeUniqueSku).WithMessage("SKU already exists");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .LessThanOrEqualTo(1000000).WithMessage("Price must not exceed 1,000,000");

        RuleFor(x => x.CreatedBy)
            .GreaterThan(0).WithMessage("CreatedBy must be a valid user ID");
    }

    private async Task<bool> BeUniqueSku(string sku, CancellationToken cancellationToken)
    {
        return !await _productService.SkuExistsAsync(sku);
    }
}