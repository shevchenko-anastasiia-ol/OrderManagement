using FluentValidation;
using FluentValidation.Results;

namespace Catalog.Domain.Exceptions;

public class CustomValidationException : ValidationException
{
    public CustomValidationException(IEnumerable<ValidationFailure> failures)
        : base(failures) { }
}