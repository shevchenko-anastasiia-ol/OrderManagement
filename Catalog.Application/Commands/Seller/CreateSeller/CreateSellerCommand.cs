using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.ValueObjects;

namespace Catalog.Application.Commands.Seller.CreateSeller;

public class CreateSellerCommand : ICommand<Domain.Entities.Seller>
{
    public string Name { get; init; } = default!;
    public Email Email { get; init; } = default!;
    public Phone Phone { get; init; } = default!;
    public string UserId { get; init; } = default!;
}
