using Catalog.Application.Commands.Product.UpdateProductPrice;
using FluentValidation;

namespace Catalog.Application.Validators.Product;

public class UpdateProductPriceCommandValidator : AbstractValidator<UpdateProductPriceCommand>
{
    public UpdateProductPriceCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId є обов’язковим.");

        RuleFor(x => x.NewPrice)
            .NotNull()
            .WithMessage("Нова ціна є обов’язковою.")
            .Must(price => price.Amount > 0)
            .WithMessage("Нова ціна повинна бути більшою за 0.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId є обов’язковим.");
    }
}