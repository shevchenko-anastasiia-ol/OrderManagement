using FluentValidation;
using WarehouseBLL.DTOs.WarehouseDetail;

namespace WarehouseBLL.Validators.WarehouseDetails;

public class WarehouseDetailsUpdateDtoValidator : AbstractValidator<WarehouseDetailUpdateDto>
{
    public WarehouseDetailsUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Invalid warehouse details ID");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Address));

        RuleFor(x => x.Manager)
            .MaximumLength(200).WithMessage("Manager name must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Manager));

        RuleFor(x => x.UpdatedBy)
            .GreaterThan(0).WithMessage("UpdatedBy must be a valid user ID");
    }
}