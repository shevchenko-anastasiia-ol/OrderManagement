using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.ValueObjects;

namespace Catalog.Application.Commands.Seller.UpdateSeller;

public class UpdateSellerCommand : ICommand<Domain.Entities.Seller>
{
    public string SellerId { get; init; } = default!;
    public string Name { get; init; } = default!;
    public Email Email { get; init; } = default!;
    public Phone Phone { get; init; } = default!;
    public string UserId { get; init; } = default!;
}