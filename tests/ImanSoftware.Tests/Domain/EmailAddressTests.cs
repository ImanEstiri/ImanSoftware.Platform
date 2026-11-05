using ImanSoftware.Domain;
using ImanSoftware.Outcomes;

namespace ImanSoftware.Tests.Domain;

public class EmailAddressTests
{
    [Theory]
    [InlineData("a@b.c")]
    [InlineData("user.name@example.com")]
    [InlineData("UPPER@DOMAIN.CO")]
    [InlineData("a@b.c.d.e")]
    public void Ctor_WithValidEmail_AcceptsValue(string value)
    {
        Assert.Equal(value, new EmailAddress(value).Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("a@b")]
    [InlineData("a@b.")]
    [InlineData("a b@c.d")]
    [InlineData("@b.c")]
    [InlineData("a@b c.d")]
    [InlineData("plain")]
    public void Ctor_WithInvalidEmail_ThrowsValidationException(string value)
    {
        Assert.Throws<ValidationException>(() => new EmailAddress(value));
    }

    [Fact]
    public void Ctor_WithNull_ThrowsWithRequiredMessage()
    {
        var exception = Assert.Throws<ValidationException>(() => new EmailAddress(null!));

        Assert.Equal("Email address cannot be null or empty.", exception.Message);
        Assert.Contains("Email is required.", exception.Errors);
    }

    [Fact]
    public void Ctor_WithBadFormat_ThrowsWithFormatMessage()
    {
        var exception = Assert.Throws<ValidationException>(() => new EmailAddress("invalid"));

        Assert.Equal("'invalid' is not a valid email address.", exception.Message);
        Assert.Contains("Invalid email format.", exception.Errors);
    }

    [Fact]
    public void ImplicitOperator_ToString_ReturnsValue()
    {
        string value = new EmailAddress("a@b.c");

        Assert.Equal("a@b.c", value);
    }

    [Fact]
    public void ExplicitOperator_FromString_CreatesInstance()
    {
        var email = (EmailAddress)"a@b.c";

        Assert.Equal("a@b.c", email.Value);
    }

    [Fact]
    public void ExplicitOperator_FromInvalidString_ThrowsValidationException()
    {
        Assert.Throws<ValidationException>(() => (EmailAddress)"nope");
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        Assert.Equal("a@b.c", new EmailAddress("a@b.c").ToString());
    }
}
