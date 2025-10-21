using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.ValueObjects;

namespace Catalog.Application.Commands.Product.UpdateProductPrice;

public class UpdateProductPriceCommand : ICommand<Domain.Entities.Product>
{
    public string ProductId { get; init; } = default!;
    public Money NewPrice { get; init; } = default!;
    public string UserId { get; init; } = default!;
}