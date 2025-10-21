using Catalog.Application.Commands.Category.DeleteCategory;
using FluentValidation;

namespace Catalog.Application.Validators.Category;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("CategoryId is required to delete a category.")
            .Matches("^[a-fA-F0-9]{24}$")
            .WithMessage("CategoryId must be a valid MongoDB ObjectId.");
    }
}