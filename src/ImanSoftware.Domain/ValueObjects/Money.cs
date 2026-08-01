// مسیر: ValueObjects/Money.cs
using ImanSoftware.Outcomes;

namespace ImanSoftware.Domain;

public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ValidationException("Amount cannot be negative.", ["Amount must be zero or positive."]);

        if (string.IsNullOrWhiteSpace(currency))
            throw new ValidationException("Currency cannot be null or empty.", ["Currency is required."]);

        if (currency.Length != 3)
            throw new ValidationException("Currency must be a 3-letter ISO code.", ["Invalid currency format."]);

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new ValidationException($"Cannot add money with different currencies: {Currency} vs {other.Currency}.",
                ["Currency mismatch."]);

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new ValidationException($"Cannot subtract money with different currencies: {Currency} vs {other.Currency}.",
                ["Currency mismatch."]);

        return new Money(Amount - other.Amount, Currency);
    }

    public override string ToString() => $"{Amount:F2} {Currency}";

    public static Money Zero(string currency) => new(0, currency);
}