using Catalog.Application.Commands.Seller.DeleteSeller;
using FluentValidation;
using MongoDB.Bson;

namespace Catalog.Application.Validators.Seller;

public class DeleteSellerCommandValidator : AbstractValidator<DeleteSellerCommand>
{
    public DeleteSellerCommandValidator()
    {
        RuleFor(x => x.SellerId)
            .NotEmpty().WithMessage("SellerId is required.")
            .Must(BeValidObjectId).WithMessage("SellerId must be a valid Mongo ObjectId.");
    }

    private bool BeValidObjectId(string id)
    {
        return ObjectId.TryParse(id, out _);
    }
}