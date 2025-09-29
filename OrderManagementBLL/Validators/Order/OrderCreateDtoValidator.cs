using FluentValidation;
using OrderManagementBLL.DTOs.Order;

namespace OrderManagementBLL.Validators;

public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateDtoValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("CustomerId must be greater than 0.");

        RuleFor(x => x.OrderDate)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Order date cannot be in the future.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .MaximumLength(50).WithMessage("Status cannot exceed 50 characters.");

        RuleFor(x => x.TotalAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Total amount must be non-negative.");

        RuleFor(x => x.OrderItems)
            .NotEmpty().WithMessage("Order must contain at least one item.");

        RuleForEach(x => x.OrderItems).SetValidator(new OrderItemCreateDtoValidator());

        RuleFor(x => x.IdempotencyToken)
            .NotEmpty().WithMessage("Idempotency token is required.");
    }
}