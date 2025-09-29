using FluentValidation;
using OrderManagementBLL.DTOs.OrderItem;

namespace OrderManagementBLL.Validators;

public class OrderItemUpdateDtoValidator : AbstractValidator<OrderItemUpdateDto>
{
    public OrderItemUpdateDtoValidator()
    {
        RuleFor(x => x.OrderItemId)
            .GreaterThan(0).WithMessage("OrderItemId must be greater than 0.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).When(x => x.Quantity.HasValue)
            .WithMessage("Quantity must be greater than 0 if specified.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).When(x => x.UnitPrice.HasValue)
            .WithMessage("UnitPrice must be non-negative if specified.");

        RuleFor(x => x.UpdatedBy)
            .NotEmpty().WithMessage("UpdatedBy is required.");

        RuleFor(x => x.RowVer)
            .NotNull().WithMessage("RowVer is required for concurrency control.");
    }
}