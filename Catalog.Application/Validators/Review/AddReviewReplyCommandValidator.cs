using Catalog.Application.Commands.Review.AddReviewReply;
using FluentValidation;
using MongoDB.Bson;

namespace Catalog.Application.Validators.Review;

public class AddReviewReplyCommandValidator : AbstractValidator<AddReviewReplyCommand>
{
    public AddReviewReplyCommandValidator()
    {
        RuleFor(x => x.ReviewId)
            .NotEmpty().WithMessage("ReviewId is required.")
            .Must(BeValidObjectId).WithMessage("ReviewId must be a valid Mongo ObjectId.");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required.")
            .MinimumLength(2).WithMessage("Author must be at least 2 characters long.");

        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Text is required.")
            .MinimumLength(5).WithMessage("Text must be at least 5 characters long.")
            .MaximumLength(1000).WithMessage("Text must not exceed 1000 characters.");
    }

    private bool BeValidObjectId(string id)
    {
        return ObjectId.TryParse(id, out _);
    }
}