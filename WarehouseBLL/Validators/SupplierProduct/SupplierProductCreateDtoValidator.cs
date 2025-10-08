using FluentValidation;
using WarehouseBLL.DTOs.SupplierProduct;
using WarehouseBLL.Services.Interfaces;

namespace WarehouseBLL.Validators.SupplierProduct;

public class SupplierProductCreateDtoValidator : AbstractValidator<SupplierProductCreateDto>
{
    private readonly ISupplierService _supplierService;
    private readonly IProductService _productService;

    public SupplierProductCreateDtoValidator(ISupplierService supplierService, IProductService productService)
    {
        _supplierService = supplierService;
        _productService = productService;

        RuleFor(x => x.SupplierId)
            .GreaterThan(0).WithMessage("Invalid supplier ID")
            .MustAsync(SupplierExists).WithMessage("Supplier does not exist");

        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Invalid product ID")
            .MustAsync(ProductExists).WithMessage("Product does not exist");

        RuleFor(x => x.CreatedBy)
            .GreaterThan(0).WithMessage("CreatedBy must be a valid user ID");
    }

    private async Task<bool> SupplierExists(int supplierId, CancellationToken cancellationToken)
    {
        return await _supplierService.SupplierExistsAsync(supplierId);
    }

    private async Task<bool> ProductExists(int productId, CancellationToken cancellationToken)
    {
        return await _productService.ProductExistsAsync(productId);
    }
}
