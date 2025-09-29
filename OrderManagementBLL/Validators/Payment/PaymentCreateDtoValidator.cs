using FluentValidation;
using OrderManagementBLL.DTOs.Payment;

namespace OrderManagementBLL.Validators;

public class PaymentCreateDtoValidator : AbstractValidator<PaymentCreateDto>
{
    public PaymentCreateDtoValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage("OrderId must be greater than 0.");

        RuleFor(x => x.PaymentDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("PaymentDate cannot be in the future.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("PaymentMethod is required.")
            .MaximumLength(50).WithMessage("PaymentMethod cannot exceed 50 characters.");

        RuleFor(x => x.PaymentStatus)
            .NotEmpty().WithMessage("PaymentStatus is required.")
            .Must(s => new[] { "Pending", "Completed", "Failed" }.Contains(s))
            .WithMessage("PaymentStatus must be either Pending, Completed, or Failed.");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("CreatedBy is required.");
    }
}
