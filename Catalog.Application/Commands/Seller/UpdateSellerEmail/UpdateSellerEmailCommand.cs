using Catalog.Application.Interfaces.Commands;

namespace Catalog.Application.Commands.Seller.UpdateSellerEmail;

public class UpdateSellerEmailCommand : ICommand
{
    public string SellerId { get; init; } = default!;
    public string NewEmail { get; init; } = default!;
}