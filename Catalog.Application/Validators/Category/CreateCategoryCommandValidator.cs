using Catalog.Application.Commands.Category;
using FluentValidation;

namespace Catalog.Application.Validators.Category;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
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