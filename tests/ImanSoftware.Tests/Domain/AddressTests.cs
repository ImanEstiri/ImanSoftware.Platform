using ImanSoftware.Domain;
using ImanSoftware.Outcomes;

namespace ImanSoftware.Tests.Domain;

public class AddressTests
{
    [Fact]
    public void Ctor_WithValidData_SetsAllProperties()
    {
        var address = new Address("Main St", "Tehran", "Tehran", "12345", "Iran");

        Assert.Equal("Main St", address.Street);
        Assert.Equal("Tehran", address.City);
        Assert.Equal("Tehran", address.State);
        Assert.Equal("12345", address.PostalCode);
        Assert.Equal("Iran", address.Country);
    }

    [Fact]
    public void Ctor_WithNullStateAndPostalCode_CoercesToEmpty()
    {
        var address = new Address("Main St", "Tehran", null, null, "Iran");

        Assert.Equal(string.Empty, address.State);
        Assert.Equal(string.Empty, address.PostalCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_WithInvalidStreet_ThrowsValidationException(string? street)
    {
        var exception = Assert.Throws<ValidationException>(() => new Address(street!, "Tehran", null, null, "Iran"));

        Assert.Equal("Street cannot be null or empty.", exception.Message);
        Assert.Contains("Street is required.", exception.Errors);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_WithInvalidCity_ThrowsValidationException(string? city)
    {
        var exception = Assert.Throws<ValidationException>(() => new Address("Main St", city!, null, null, "Iran"));

        Assert.Equal("City cannot be null or empty.", exception.Message);
        Assert.Contains("City is required.", exception.Errors);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_WithInvalidCountry_ThrowsValidationException(string? country)
    {
        var exception = Assert.Throws<ValidationException>(() => new Address("Main St", "Tehran", null, null, country!));

        Assert.Equal("Country cannot be null or empty.", exception.Message);
        Assert.Contains("Country is required.", exception.Errors);
    }

    [Fact]
    public void ToString_JoinsNonEmptyPartsWithComma()
    {
        var address = new Address("Main St", "Tehran", "Tehran", "12345", "Iran");

        Assert.Equal("Main St, Tehran, Tehran, 12345, Iran", address.ToString());
    }

    [Fact]
    public void ToString_SkipsEmptyStateAndPostalCode()
    {
        var address = new Address("Main St", "Tehran", null, null, "Iran");

        Assert.Equal("Main St, Tehran, Iran", address.ToString());
    }

    [Fact]
    public void RecordEquality_ComparesAllFiveParts()
    {
        var first = new Address("St", "City", "S", "P", "Country");
        var second = new Address("St", "City", "S", "P", "Country");

        Assert.Equal(first, second);
        Assert.NotEqual(first, new Address("Other", "City", "S", "P", "Country"));
    }
}
