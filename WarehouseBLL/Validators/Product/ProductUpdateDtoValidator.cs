using FluentValidation;
using WarehouseBLL.DTOs.Product;

namespace WarehouseBLL.Validators.Product;

public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
{
    public ProductUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Invalid product ID");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters")
            .MinimumLength(3).WithMessage("Product name must be at least 3 characters");

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required")
            .MaximumLength(100).WithMessage("SKU must not exceed 100 characters")
            .Matches("^[A-Za-z0-9-_]+$").WithMessage("SKU can only contain alphanumeric characters, hyphens, and underscores");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .LessThanOrEqualTo(1000000).WithMessage("Price must not exceed 1,000,000");

        RuleFor(x => x.UpdatedBy)
            .GreaterThan(0).WithMessage("UpdatedBy must be a valid user ID");
    }
}