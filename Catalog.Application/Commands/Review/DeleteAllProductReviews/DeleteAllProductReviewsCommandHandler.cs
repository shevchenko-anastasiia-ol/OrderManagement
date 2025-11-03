using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Commands.Review.DeleteAllProductReviews;

public class DeleteAllProductReviewsCommandHandler : ICommandHandler<DeleteAllProductReviewsCommand, long>
{
    private readonly IReviewService _reviewService;

    public DeleteAllProductReviewsCommandHandler(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<long> Handle(DeleteAllProductReviewsCommand request, CancellationToken cancellationToken)
    {
        return await _reviewService.DeleteAllProductReviewsAsync(request.ProductId, cancellationToken);
    }
}