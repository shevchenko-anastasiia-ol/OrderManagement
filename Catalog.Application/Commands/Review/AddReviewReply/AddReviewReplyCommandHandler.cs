using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Commands.Review.AddReviewReply;

public class AddReviewReplyCommandHandler : ICommandHandler<AddReviewReplyCommand, Domain.Entities.Review>
{
    private readonly IReviewService _reviewService;

    public AddReviewReplyCommandHandler(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<Domain.Entities.Review> Handle(AddReviewReplyCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewService.GetByIdAsync(request.ReviewId, cancellationToken);
        review.AddReply(request.Author, request.Text);
        await _reviewService.UpdateAsync(review, cancellationToken);
        return review;
    }
}