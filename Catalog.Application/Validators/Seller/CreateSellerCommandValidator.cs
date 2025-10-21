using FluentValidation;
using MongoDB.Bson;
using Catalog.Application.Commands.Seller;
using Catalog.Application.Commands.Seller.CreateSeller;

namespace Catalog.Application.Validators.Seller;

public class CreateSellerCommandValidator : AbstractValidator<CreateSellerCommand>
{
    public CreateSellerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Seller name is required.")
            .MinimumLength(2).WithMessage("Seller name must be at least 2 characters long.");
        
        RuleFor(x => x.Email)
            .NotNull().WithMessage("Email is required.")
            .Must(email => email != null && email.IsValid()) 
            .WithMessage("Invalid email format.");
        
        RuleFor(x => x.Phone)
            .NotNull().WithMessage("Phone is required.")
            .Must(phone => phone != null && phone.IsValid()) 
            .WithMessage("Invalid phone number format.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .Must(BeValidObjectId).WithMessage("Invalid UserId format. Must be a valid Mongo ObjectId.");
    }
    private bool BeValidObjectId(string userId)
    {
        return ObjectId.TryParse(userId, out _);
    }
}