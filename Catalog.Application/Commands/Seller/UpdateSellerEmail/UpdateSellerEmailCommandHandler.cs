using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;
using MediatR;

namespace Catalog.Application.Commands.Seller.UpdateSellerEmail;

public class UpdateSellerEmailCommandHandler : ICommandHandler<UpdateSellerEmailCommand>
{
    private readonly ISellerService _sellerService;

    public UpdateSellerEmailCommandHandler(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    public async Task<Unit> Handle(UpdateSellerEmailCommand request, CancellationToken cancellationToken)
    {
        await _sellerService.UpdateEmailAsync(request.SellerId, request.NewEmail, cancellationToken);
        return Unit.Value;
    }
}