using Catalog.Application.Commands.Review.CreateReview;
using FluentValidation;
using MongoDB.Bson;

namespace Catalog.Application.Validators.Review;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required.")
            .Must(BeValidObjectId).WithMessage("ProductId must be a valid Mongo ObjectId.");
        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .Must(BeValidObjectId).WithMessage("UserId must be a valid Mongo ObjectId.");
        
        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required.")
            .MinimumLength(2).WithMessage("Author must be at least 2 characters long.");
        
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
        
        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment is required.")
            .MinimumLength(5).WithMessage("Comment must be at least 5 characters long.")
            .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.");
    }

    private bool BeValidObjectId(string id)
    {
        return ObjectId.TryParse(id, out _);
    }
}