using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Commands.Review.UpdateReview;

public class UpdateReviewCommandHandler : ICommandHandler<UpdateReviewCommand, Domain.Entities.Review>
{
    private readonly IReviewService _reviewService;

    public UpdateReviewCommandHandler(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<Domain.Entities.Review> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewService.GetByIdAsync(request.ReviewId, cancellationToken);
        review.Update(request.Rating, request.Comment, request.UserId);
        await _reviewService.UpdateAsync(review, cancellationToken);
        return review;
    }
}