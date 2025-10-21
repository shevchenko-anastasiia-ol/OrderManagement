using Catalog.Application.Commands.Seller.UpdateSeller;
using FluentValidation;
using MongoDB.Bson;

namespace Catalog.Application.Validators.Seller;

public class UpdateSellerCommandValidator : AbstractValidator<UpdateSellerCommand>
{
    public UpdateSellerCommandValidator()
    {
        RuleFor(x => x.SellerId)
            .NotEmpty().WithMessage("SellerId is required.")
            .Must(BeValidObjectId).WithMessage("SellerId must be a valid Mongo ObjectId.");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters long.");
        
        RuleFor(x => x.Email)
            .NotNull().WithMessage("Email is required.")
            .Must(email => email.IsValid()).WithMessage("Invalid email format.");
        
        RuleFor(x => x.Phone)
            .NotNull().WithMessage("Phone is required.")
            .Must(phone => phone.IsValid()).WithMessage("Invalid phone format.");
        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .Must(BeValidObjectId).WithMessage("UserId must be a valid Mongo ObjectId.");
    }

    private bool BeValidObjectId(string id)
    {
        return ObjectId.TryParse(id, out _);
    }
}