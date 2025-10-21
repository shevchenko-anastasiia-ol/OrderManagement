using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;
using MediatR;

namespace Catalog.Application.Commands.Review.DeleteReview;

public class DeleteReviewCommandHandler : ICommandHandler<DeleteReviewCommand>
{
    private readonly IReviewService _reviewService;

    public DeleteReviewCommandHandler(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        await _reviewService.DeleteAsync(request.ReviewId, cancellationToken);
        return Unit.Value;
    }
}