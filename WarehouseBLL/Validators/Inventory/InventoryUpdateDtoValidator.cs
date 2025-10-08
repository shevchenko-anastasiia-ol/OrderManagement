using FluentValidation;
using WarehouseBLL.DTOs.Inventory;

namespace WarehouseBLL.Validators.Inventory;

public class InventoryUpdateDtoValidator : AbstractValidator<InventoryUpdateDto>
{
    public InventoryUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Invalid inventory ID");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative")
            .LessThanOrEqualTo(1000000).WithMessage("Quantity must not exceed 1,000,000");

        RuleFor(x => x.UpdatedBy)
            .GreaterThan(0).WithMessage("UpdatedBy must be a valid user ID");
    }
}