using System.Text.RegularExpressions;
using Catalog.Domain.Common;
using Catalog.Domain.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Domain.ValueObjects;

[BsonIgnoreExtraElements]
public sealed class Phone : ValueObject
{
    private static readonly Regex PhoneRegex = new(@"^\+?\d{10,15}$", RegexOptions.Compiled);

    [BsonElement("value")]
    public string Value { get; }

    private Phone() { Value = string.Empty; }

    public Phone(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("The phone cannot be empty.");
        var cleanedPhone = value.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        if (!PhoneRegex.IsMatch(cleanedPhone))
            throw new DomainException($"Invalid phone format: {value}");
        Value = cleanedPhone;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
    public static implicit operator string(Phone phone) => phone.Value;
}