using FluentValidation;
using WarehouseBLL.DTOs.Inventory;

namespace WarehouseBLL.Validators.Inventory;

public class InventoryAdjustDtoValidator : AbstractValidator<InventoryAdjustDto>
{
    public InventoryAdjustDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Invalid inventory ID");

        RuleFor(x => x.QuantityChange)
            .NotEqual(0).WithMessage("Quantity change must not be zero")
            .GreaterThanOrEqualTo(-1000000).WithMessage("Quantity change too large (negative)")
            .LessThanOrEqualTo(1000000).WithMessage("Quantity change too large (positive)");

        RuleFor(x => x.UpdatedBy)
            .GreaterThan(0).WithMessage("UpdatedBy must be a valid user ID");
    }
}