using Catalog.Application.Interfaces.Commands;

namespace Catalog.Application.Commands.Seller.UpdateSellerPhone;

public class UpdateSellerPhoneCommand : ICommand
{
    public string SellerId { get; init; } = default!;
    public string NewPhone { get; init; } = default!;
}