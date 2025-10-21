using Catalog.Application.Interfaces.Commands;

namespace Catalog.Application.Commands.Review.DeleteReview;

public class DeleteReviewCommand : ICommand
{
    public string ReviewId { get; init; } = default!;
}