using FluentValidation;
using OrderManagementBLL.DTOs.Order;

namespace OrderManagementBLL.Validators;

public class OrderUpdateDtoValidator : AbstractValidator<OrderUpdateDto>
{
    public OrderUpdateDtoValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage("OrderId must be greater than 0.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .MaximumLength(50).WithMessage("Status cannot exceed 50 characters.");

        RuleFor(x => x.TotalAmount)
            .GreaterThanOrEqualTo(0).When(x => x.TotalAmount.HasValue)
            .WithMessage("Total amount must be non-negative.");

        RuleFor(x => x.OrderItems)
            .NotNull().WithMessage("OrderItems cannot be null.");

        RuleForEach(x => x.OrderItems).SetValidator(new OrderItemUpdateDtoValidator());

        RuleFor(x => x.UpdatedBy)
            .NotEmpty().WithMessage("UpdatedBy is required.");

        RuleFor(x => x.RowVer)
            .NotNull().WithMessage("RowVer is required for concurrency control.");
    }
}