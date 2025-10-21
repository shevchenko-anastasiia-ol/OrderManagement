using Catalog.Application.Commands.Review.UpdateReview;
using FluentValidation;
using MongoDB.Bson;

namespace Catalog.Application.Validators.Review;

public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId)
            .NotEmpty().WithMessage("ReviewId is required.")
            .Must(BeValidObjectId).WithMessage("ReviewId must be a valid Mongo ObjectId.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .Must(BeValidObjectId).WithMessage("UserId must be a valid Mongo ObjectId.");

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