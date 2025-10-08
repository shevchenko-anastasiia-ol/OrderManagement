using FluentValidation;
using WarehouseBLL.DTOs.Inventory;
using WarehouseBLL.Services.Interfaces;

namespace WarehouseBLL.Validators.Inventory;

public class InventoryCreateDtoValidator : AbstractValidator<InventoryCreateDto>
{
    private readonly IWarehouseService _warehouseService;
    private readonly IProductService _productService;

    public InventoryCreateDtoValidator(IWarehouseService warehouseService, IProductService productService)
    {
        _warehouseService = warehouseService;
        _productService = productService;

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0).WithMessage("Invalid warehouse ID")
            .MustAsync(WarehouseExists).WithMessage("Warehouse does not exist");

        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Invalid product ID")
            .MustAsync(ProductExists).WithMessage("Product does not exist");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative")
            .LessThanOrEqualTo(1000000).WithMessage("Quantity must not exceed 1,000,000");

        RuleFor(x => x.CreatedBy)
            .GreaterThan(0).WithMessage("CreatedBy must be a valid user ID");
    }

    private async Task<bool> WarehouseExists(int warehouseId, CancellationToken cancellationToken)
    {
        return await _warehouseService.WarehouseExistsAsync(warehouseId);
    }

    private async Task<bool> ProductExists(int productId, CancellationToken cancellationToken)
    {
        return await _productService.ProductExistsAsync(productId);
    }
}