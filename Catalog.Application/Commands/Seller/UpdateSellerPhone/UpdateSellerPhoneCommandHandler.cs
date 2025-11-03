using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;
using MediatR;

namespace Catalog.Application.Commands.Seller.UpdateSellerPhone;

public class UpdateSellerPhoneCommandHandler : ICommandHandler<UpdateSellerPhoneCommand>
{
    private readonly ISellerService _sellerService;

    public UpdateSellerPhoneCommandHandler(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    public async Task<Unit> Handle(UpdateSellerPhoneCommand request, CancellationToken cancellationToken)
    {
        await _sellerService.UpdatePhoneAsync(request.SellerId, request.NewPhone, cancellationToken);
        return Unit.Value;
    }
}