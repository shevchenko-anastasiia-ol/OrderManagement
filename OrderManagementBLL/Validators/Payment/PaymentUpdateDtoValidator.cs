using FluentValidation;
using OrderManagementBLL.DTOs.Payment;

namespace OrderManagementBLL.Validators;

public class PaymentUpdateDtoValidator : AbstractValidator<PaymentUpdateDto>
{
    public PaymentUpdateDtoValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0).WithMessage("PaymentId must be greater than 0.");

        RuleFor(x => x.PaymentStatus)
            .NotEmpty().WithMessage("PaymentStatus is required.")
            .Must(s => new[] { "Pending", "Completed", "Failed" }.Contains(s))
            .When(x => x.PaymentStatus != null)
            .WithMessage("PaymentStatus must be either Pending, Completed, or Failed.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).When(x => x.Amount.HasValue)
            .WithMessage("Amount must be greater than 0 if specified.");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().When(x => x.PaymentMethod != null)
            .WithMessage("PaymentMethod cannot be empty if specified.")
            .MaximumLength(50).When(x => x.PaymentMethod != null)
            .WithMessage("PaymentMethod cannot exceed 50 characters.");

        RuleFor(x => x.PaymentDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.PaymentDate.HasValue)
            .WithMessage("PaymentDate cannot be in the future.");

        RuleFor(x => x.UpdatedBy)
            .NotEmpty().WithMessage("UpdatedBy is required.");
    }
}