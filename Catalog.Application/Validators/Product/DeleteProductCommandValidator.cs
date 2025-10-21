using Catalog.Application.Commands.Product.DeleteProduct;
using FluentValidation;

namespace Catalog.Application.Validators.Product;

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId є обов’язковим.");

        RuleFor(x => x.DeleteReviews)
            .NotNull()
            .WithMessage("DeleteReviews не може бути null."); 
    }
}