using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Commands.Review.CreateReview;

public class CreateReviewCommandHandler : ICommandHandler<CreateReviewCommand, Domain.Entities.Review>
{
    private readonly IReviewService _reviewService;

    public CreateReviewCommandHandler(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<Domain.Entities.Review> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = new Domain.Entities.Review(
            request.ProductId,
            request.Author,
            request.Rating,
            request.Comment,
            request.UserId);

        await _reviewService.CreateAsync(review, cancellationToken);
        return review;
    }
}