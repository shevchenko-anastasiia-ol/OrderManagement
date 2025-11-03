using Catalog.Application.Interfaces.Commands;

namespace Catalog.Application.Commands.Product.AddCategoryToProduct;

public class AddCategoryToProductCommand : ICommand
{
    public string ProductId { get; init; } = default!;
    public string CategoryId { get; init; } = default!;
}