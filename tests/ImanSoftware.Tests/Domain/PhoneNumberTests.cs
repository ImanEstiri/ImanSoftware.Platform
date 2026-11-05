using ImanSoftware.Domain;
using ImanSoftware.Outcomes;

namespace ImanSoftware.Tests.Domain;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("12")]
    [InlineData("+1234567890")]
    [InlineData("989121234567")]
    public void Ctor_WithValidPhone_AcceptsValue(string value)
    {
        Assert.Equal(value, new PhoneNumber(value).Value);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("0123")]
    [InlineData("12a")]
    [InlineData("12 34")]
    [InlineData("+0123")]
    [InlineData("+1234567890123456")]
    public void Ctor_WithInvalidPhone_ThrowsValidationException(string value)
    {
        Assert.Throws<ValidationException>(() => new PhoneNumber(value));
    }

    [Fact]
    public void Ctor_WithNull_ThrowsWithRequiredMessage()
    {
        var exception = Assert.Throws<ValidationException>(() => new PhoneNumber(null!));

        Assert.Equal("Phone number cannot be null or empty.", exception.Message);
        Assert.Contains("Phone is required.", exception.Errors);
    }

    [Fact]
    public void Ctor_WithBadFormat_ThrowsWithFormatMessage()
    {
        var exception = Assert.Throws<ValidationException>(() => new PhoneNumber("abc"));

        Assert.Equal("'abc' is not a valid phone number.", exception.Message);
        Assert.Contains("Invalid phone format.", exception.Errors);
    }

    [Fact]
    public void ImplicitOperator_ToString_ReturnsValue()
    {
        string value = new PhoneNumber("+1234567890");

        Assert.Equal("+1234567890", value);
    }

    [Fact]
    public void ExplicitOperator_FromString_CreatesInstance()
    {
        var phone = (PhoneNumber)"12";

        Assert.Equal("12", phone.Value);
    }

    [Fact]
    public void ExplicitOperator_FromInvalidString_ThrowsValidationException()
    {
        Assert.Throws<ValidationException>(() => (PhoneNumber)"0123");
    }
}
