using Catalog.Application.Interfaces.Commands;

namespace Catalog.Application.Commands.Product.RemoveCategoryFromProduct;

public class RemoveCategoryFromProductCommand : ICommand
{
    public string ProductId { get; init; } = default!;
    public string CategoryId { get; init; } = default!;
}