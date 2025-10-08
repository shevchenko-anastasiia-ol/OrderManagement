using FluentValidation;
using WarehouseBLL.DTOs.WarehouseDetail;
using WarehouseBLL.Services.Interfaces;

namespace WarehouseBLL.Validators.WarehouseDetails;

public class WarehouseDetailsCreateDtoValidator : AbstractValidator<WarehouseDetailCreateDto>
{
    private readonly IWarehouseService _warehouseService;

    public WarehouseDetailsCreateDtoValidator(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0).WithMessage("Invalid warehouse ID")
            .MustAsync(WarehouseExists).WithMessage("Warehouse does not exist");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Address));

        RuleFor(x => x.Manager)
            .MaximumLength(200).WithMessage("Manager name must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Manager));

        RuleFor(x => x.CreatedBy)
            .GreaterThan(0).WithMessage("CreatedBy must be a valid user ID");
    }

    private async Task<bool> WarehouseExists(int warehouseId, CancellationToken cancellationToken)
    {
        return await _warehouseService.WarehouseExistsAsync(warehouseId);
    }
}