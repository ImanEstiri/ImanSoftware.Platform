using ImanSoftware.Domain;
using ImanSoftware.Outcomes;

namespace ImanSoftware.Tests.Domain;

public class MoneyTests
{
    [Fact]
    public void Ctor_WithValidAmountAndCurrency_SetsUppercasedCurrency()
    {
        var money = new Money(10.5m, "usd");

        Assert.Equal(10.5m, money.Amount);
        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void Ctor_WithZeroAmount_IsAllowed()
    {
        Assert.Equal(0m, new Money(0m, "EUR").Amount);
    }

    [Fact]
    public void Ctor_WithNegativeAmount_ThrowsValidationException()
    {
        var exception = Assert.Throws<ValidationException>(() => new Money(-1m, "USD"));

        Assert.Equal("Amount cannot be negative.", exception.Message);
        Assert.Contains("Amount must be zero or positive.", exception.Errors);
    }

    [Fact]
    public void Ctor_WithEmptyCurrency_ThrowsValidationException()
    {
        var exception = Assert.Throws<ValidationException>(() => new Money(1m, " "));

        Assert.Equal("Currency cannot be null or empty.", exception.Message);
    }

    [Fact]
    public void Ctor_WithNonThreeLetterCurrency_ThrowsValidationException()
    {
        var exception = Assert.Throws<ValidationException>(() => new Money(1m, "USDX"));

        Assert.Equal("Currency must be a 3-letter ISO code.", exception.Message);
    }

    [Fact]
    public void Add_WithSameCurrency_SumsAmounts()
    {
        var result = new Money(10m, "USD").Add(new Money(5.25m, "USD"));

        Assert.Equal(15.25m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Add_WithDifferentCurrency_ThrowsValidationException()
    {
        var exception = Assert.Throws<ValidationException>(() => new Money(10m, "USD").Add(new Money(5m, "EUR")));

        Assert.Equal("Cannot add money with different currencies: USD vs EUR.", exception.Message);
    }

    [Fact]
    public void Subtract_WithSameCurrency_SubtractsAmounts()
    {
        var result = new Money(10m, "USD").Subtract(new Money(4m, "USD"));

        Assert.Equal(6m, result.Amount);
    }

    [Fact]
    public void Subtract_WithDifferentCurrency_ThrowsValidationException()
    {
        var exception = Assert.Throws<ValidationException>(() => new Money(10m, "USD").Subtract(new Money(4m, "EUR")));

        Assert.Equal("Cannot subtract money with different currencies: USD vs EUR.", exception.Message);
    }

    [Fact]
    public void Subtract_ResultBelowZero_ThrowsValidationException()
    {
        Assert.Throws<ValidationException>(() => new Money(5m, "USD").Subtract(new Money(10m, "USD")));
    }

    [Fact]
    public void Zero_ReturnsZeroAmountWithUppercasedCurrency()
    {
        var money = Money.Zero("irr");

        Assert.Equal(0m, money.Amount);
        Assert.Equal("IRR", money.Currency);
    }

    [Fact]
    public void RecordEquality_AfterNormalization_IsCaseInsensitiveOnCurrency()
    {
        Assert.Equal(new Money(10m, "usd"), new Money(10m, "USD"));
        Assert.NotEqual(new Money(10m, "USD"), new Money(11m, "USD"));
    }

    [Fact]
    public void ToString_EndsWithCurrencyCode()
    {
        var money = new Money(10.5m, "USD");

        Assert.EndsWith("USD", money.ToString(), StringComparison.Ordinal);
        Assert.Contains("10.5", money.ToString(), StringComparison.Ordinal);
    }
}
