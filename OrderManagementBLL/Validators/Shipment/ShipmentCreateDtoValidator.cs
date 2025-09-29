using FluentValidation;
using OrderManagementBLL.DTOs.Shipment;

namespace OrderManagementBLL.Validators;

public class ShipmentCreateDtoValidator : AbstractValidator<ShipmentCreateDto>
{
    public ShipmentCreateDtoValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage("OrderId must be greater than 0.");

        RuleFor(x => x.ShipmentDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("ShipmentDate cannot be in the far future.");

        RuleFor(x => x.Carrier)
            .NotEmpty().WithMessage("Carrier is required.")
            .MaximumLength(100).WithMessage("Carrier cannot exceed 100 characters.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .MaximumLength(50).WithMessage("Status cannot exceed 50 characters.");

        RuleFor(x => x.AddressLine1)
            .NotEmpty().WithMessage("AddressLine1 is required.")
            .MaximumLength(200).WithMessage("AddressLine1 cannot exceed 200 characters.");

        RuleFor(x => x.AddressLine2)
            .MaximumLength(200).WithMessage("AddressLine2 cannot exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.AddressLine2));

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

        RuleFor(x => x.Region)
            .NotEmpty().WithMessage("Region is required.")
            .MaximumLength(100).WithMessage("Region cannot exceed 100 characters.");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("PostalCode is required.")
            .MaximumLength(20).WithMessage("PostalCode cannot exceed 20 characters.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100).WithMessage("Country cannot exceed 100 characters.");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("CreatedBy is required.")
            .MaximumLength(50).WithMessage("CreatedBy cannot exceed 50 characters.");
    }
}
