using Catalog.Application.Commands.Product.CreateProduct;
using FluentValidation;
using MongoDB.Bson;

namespace Catalog.Application.Validators.Product;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MinimumLength(2).WithMessage("Product name must be at least 2 characters long.");
            
            RuleFor(x => x.Price)
                .NotNull().WithMessage("Price is required.")
                .Must(price => price.Amount > 0).WithMessage("Price must be greater than 0.")
                .Must(price => !string.IsNullOrWhiteSpace(price.Currency)).WithMessage("Currency is required.");
            
            RuleFor(x => x.SellerId)
                .NotEmpty().WithMessage("SellerId is required.")
                .Must(BeValidObjectId).WithMessage("SellerId must be a valid Mongo ObjectId.");
            
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .Must(BeValidObjectId).WithMessage("UserId must be a valid Mongo ObjectId.");
            
            RuleForEach(x => x.CategoryIds)
                .Must(BeValidObjectId)
                .WithMessage("Each category ID must be a valid Mongo ObjectId.");
            
            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

            RuleForEach(x => x.Specifications ?? new Dictionary<string, string>())
                .Must(pair => !string.IsNullOrWhiteSpace(pair.Key))
                .WithMessage("Specification key cannot be empty.")
                .Must(pair => !string.IsNullOrWhiteSpace(pair.Value))
                .WithMessage("Specification value cannot be empty.");
            
            RuleForEach(x => x.ImageUrls ?? Array.Empty<string>())
                .Must(BeValidUrl)
                .WithMessage("Each image URL must be a valid HTTP or HTTPS URL.");
        }

        private bool BeValidObjectId(string id)
        {
            return ObjectId.TryParse(id, out _);
        }

        private bool BeValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? result)
                   && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }