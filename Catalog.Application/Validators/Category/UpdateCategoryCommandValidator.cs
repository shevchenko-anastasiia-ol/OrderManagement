using Catalog.Application.Commands.Category.UpdateCategory;
using FluentValidation;

namespace Catalog.Application.Validators.Category;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("CategoryId is required to update a category.")
            .Matches("^[a-fA-F0-9]{24}$")
            .WithMessage("CategoryId must be a valid MongoDB ObjectId.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("The category name is required.")
            .MaximumLength(100)
            .WithMessage("The category name cannot exceed 100 characters.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");
    }
}