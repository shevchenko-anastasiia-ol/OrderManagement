using Catalog.Application.Interfaces.Commands;

namespace Catalog.Application.Commands.Review.CreateReview;

public class CreateReviewCommand : ICommand<Domain.Entities.Review>
{
    public string ProductId { get; init; } = default!;
    public string Author { get; init; } = default!;
    public int Rating { get; init; }
    public string Comment { get; init; } = default!;
    public string UserId { get; init; } = default!;
}
