using Catalog.Application.Commands.Product.UpdateProduct;
using FluentValidation;

namespace Catalog.Application.Validators.Product;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId є обов’язковим.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Назва продукту є обов’язковою.")
            .MaximumLength(200)
            .WithMessage("Назва продукту не може перевищувати 200 символів.");

        RuleFor(x => x.Price)
            .NotNull()
            .WithMessage("Ціна є обов’язковою.")
            .Must(price => price.Amount > 0)
            .WithMessage("Ціна повинна бути більшою за 0.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId є обов’язковим.");

        When(x => x.CategoryIds is not null, () =>
        {
            RuleFor(x => x.CategoryIds)
                .Must(list => list.All(id => !string.IsNullOrWhiteSpace(id)))
                .WithMessage("Кожен CategoryId повинен бути непорожнім.");
        });

        When(x => x.ImageUrls is not null, () =>
        {
            RuleForEach(x => x.ImageUrls)
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Кожен ImageUrl має бути коректною URL-адресою.");
        });

        When(x => x.ExpectedVersion.HasValue, () =>
        {
            RuleFor(x => x.ExpectedVersion)
                .GreaterThanOrEqualTo(0)
                .WithMessage("ExpectedVersion не може бути від’ємним.");
        });
    }
}