using Catalog.Application.Commands.Product.AddProductImages;
using FluentValidation;
using MongoDB.Bson;

namespace Catalog.Application.Validators.Product;

public class AddProductImagesCommandValidator : AbstractValidator<AddProductImagesCommand>
{
    public AddProductImagesCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required.")
            .Must(BeValidObjectId).WithMessage("ProductId must be a valid Mongo ObjectId.");
        
        RuleFor(x => x.ImageUrls)
            .NotEmpty().WithMessage("At least one image URL is required.");
        
        RuleForEach(x => x.ImageUrls)
            .NotEmpty().WithMessage("Image URL cannot be empty.")
            .Must(BeValidUrl).WithMessage("Invalid image URL format.");
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