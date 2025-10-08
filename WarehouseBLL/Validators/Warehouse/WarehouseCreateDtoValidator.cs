using FluentValidation;
using WarehouseBLL.DTOs.Warehouse;

namespace WarehouseBLL.Validators.Warehouse;

public class WarehouseCreateDtoValidator : AbstractValidator<WarehouseCreateDto>
{
    public WarehouseCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Warehouse name is required")
            .MaximumLength(200).WithMessage("Warehouse name must not exceed 200 characters")
            .MinimumLength(3).WithMessage("Warehouse name must be at least 3 characters");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0")
            .LessThanOrEqualTo(1000000).WithMessage("Capacity must not exceed 1,000,000");

        RuleFor(x => x.CreatedBy)
            .GreaterThan(0).WithMessage("CreatedBy must be a valid user ID");
    }
}