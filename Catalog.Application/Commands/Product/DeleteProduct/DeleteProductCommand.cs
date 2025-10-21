using Catalog.Application.Interfaces.Commands;

namespace Catalog.Application.Commands.Product.DeleteProduct;

public class DeleteProductCommand : ICommand
{
    public string ProductId { get; init; } = default!;
    public bool DeleteReviews { get; init; } = false;
}