﻿namespace Catalog.Domain.Exceptions;

public class ValidationException : DomainException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors) 
        : base("One or more validation errors")
    {
        Errors = errors;
    }

    public ValidationException(string propertyName, string errorMessage) 
        : base($"Validation error for '{propertyName}': {errorMessage}")
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        };
    }
}