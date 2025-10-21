using Catalog.Application.Commands.Review.DeleteReview;
using FluentValidation;
using MongoDB.Bson;

namespace Catalog.Application.Validators.Review;

public class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
{
    public DeleteReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId)
            .NotEmpty().WithMessage("ReviewId is required.")
            .Must(BeValidObjectId).WithMessage("ReviewId must be a valid Mongo ObjectId.");
    }

    private bool BeValidObjectId(string id)
    {
        return ObjectId.TryParse(id, out _);
    }
}