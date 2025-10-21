using Catalog.Application.Interfaces.Commands;

namespace Catalog.Application.Commands.Review.UpdateReview;

public class UpdateReviewCommand : ICommand<Domain.Entities.Review>
{
    public string ReviewId { get; init; } = default!;
    public int Rating { get; init; }
    public string Comment { get; init; } = default!;
    public string UserId { get; init; } = default!;
}