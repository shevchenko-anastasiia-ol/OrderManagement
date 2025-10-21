using System.Text.RegularExpressions;
using Catalog.Domain.Common;
using Catalog.Domain.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Domain.ValueObjects;

[BsonIgnoreExtraElements]
public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    [BsonElement("value")]
    public string Value { get; }

    private Email() { Value = string.Empty; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email cannot be empty");
        if (!EmailRegex.IsMatch(value))
            throw new DomainException($"Invalid format email: {value}");
        Value = value.ToLowerInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(Value))
            return false;
        
        return System.Text.RegularExpressions.Regex.IsMatch(
            Value,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase
        );
    }

    public override string ToString() => Value;
    public static implicit operator string(Email email) => email.Value;
}