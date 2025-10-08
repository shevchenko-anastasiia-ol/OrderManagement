using FluentValidation;
using WarehouseBLL.DTOs.Supplier;

namespace WarehouseBLL.Validators.Supplier;

public class SupplierUpdateDtoValidator : AbstractValidator<SupplierUpdateDto>
{
    public SupplierUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Invalid supplier ID");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Supplier name is required")
            .MaximumLength(200).WithMessage("Supplier name must not exceed 200 characters")
            .MinimumLength(3).WithMessage("Supplier name must be at least 3 characters");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters");

        RuleFor(x => x.ContactInfo)
            .MaximumLength(500).WithMessage("Contact info must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.ContactInfo));

        RuleFor(x => x.UpdatedBy)
            .GreaterThan(0).WithMessage("UpdatedBy must be a valid user ID");
    }
}