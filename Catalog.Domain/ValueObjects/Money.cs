using Catalog.Domain.Common;
using Catalog.Domain.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Domain.ValueObjects;

[BsonIgnoreExtraElements]
public sealed class Money : ValueObject
{
    [BsonElement("amount")]
    public decimal Amount { get; }

    [BsonElement("currency")]
    public string Currency { get; }

    private Money() { Currency = string.Empty; }

    public Money(decimal amount, string currency = "UAH")
    {
        if (amount < 0)
            throw new DomainException("The amount cannot be negative.");
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new DomainException("Currency code must consist of 3 characters");
        Amount = Math.Round(amount, 2);
        Currency = currency.ToUpperInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Unable to add different currencies: {Currency} and {other.Currency}");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Unable to subtract different currencies: {Currency} and {other.Currency}");
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal multiplier) => new Money(Amount * multiplier, Currency);

    public override string ToString() => $"{Amount:F2} {Currency}";

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator *(Money money, decimal multiplier) => money.Multiply(multiplier);
}