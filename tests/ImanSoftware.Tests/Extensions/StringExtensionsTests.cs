using ImanSoftware.Extensions;

namespace ImanSoftware.Tests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    [InlineData("a", false)]
    public void IsNullOrWhiteSpace_DelegatesToBcl(string? value, bool expected)
    {
        Assert.Equal(expected, value.IsNullOrWhiteSpace());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", false)]
    [InlineData("a", false)]
    public void IsNullOrEmpty_DelegatesToBcl(string? value, bool expected)
    {
        Assert.Equal(expected, value.IsNullOrEmpty());
    }

    [Fact]
    public void Truncate_WhenLongerThanMaxLength_TruncatesWithSuffix()
    {
        Assert.Equal("Hello...", "HelloWorld".Truncate(8));
    }

    [Fact]
    public void Truncate_WhenShorterThanOrEqualToMaxLength_ReturnsUnchanged()
    {
        Assert.Equal("Hello", "Hello".Truncate(5));
        Assert.Equal("Hi", "Hi".Truncate(8));
    }

    [Fact]
    public void Truncate_WithCustomSuffix_UsesSuffix()
    {
        Assert.Equal("He…", "HelloWorld".Truncate(3, "…"));
    }

    [Fact]
    public void Truncate_WithEmptyValue_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, string.Empty.Truncate(5));
    }

    [Fact]
    public void Truncate_WithNull_ReturnsNull()
    {
        string? value = null;

        Assert.Null(value.Truncate(5));
    }

    [Fact]
    public void Truncate_WithMaxLengthShorterThanSuffix_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => "HelloWorld".Truncate(2));
    }

    [Fact]
    public void Truncate_WithMaxLengthEqualToSuffixLength_ReturnsSuffixOnly()
    {
        Assert.Equal("...", "HelloWorld".Truncate(3));
    }

    [Theory]
    [InlineData("HelloWorld", "helloWorld")]
    [InlineData("ABC", "aBC")]
    [InlineData("already", "already")]
    [InlineData("", "")]
    public void ToCamelCase_LowersOnlyFirstChar(string value, string expected)
    {
        Assert.Equal(expected, value.ToCamelCase());
    }

    [Theory]
    [InlineData("HelloWorld", "hello_world")]
    [InlineData("Version2Update", "version2_update")]
    [InlineData("HELLO", "hello")]
    [InlineData("hello", "hello")]
    public void ToSnakeCase_SplitsOnUpperBoundaries(string value, string expected)
    {
        Assert.Equal(expected, value.ToSnakeCase());
    }

    [Theory]
    [InlineData("HelloWorld", "hello-world")]
    [InlineData("Version2Update", "version2-update")]
    [InlineData("hello", "hello")]
    public void ToKebabCase_SplitsOnUpperBoundaries(string value, string expected)
    {
        Assert.Equal(expected, value.ToKebabCase());
    }

    [Theory]
    [InlineData("a@b.c", true)]
    [InlineData("a@b", true)]
    [InlineData("not-an-email", false)]
    [InlineData(" a@b.c ", false)]
    [InlineData("", false)]
    [InlineData("a@b@c.d", false)]
    public void IsEmail_UsesMailAddressRoundTrip(string value, bool expected)
    {
        Assert.Equal(expected, value.IsEmail());
    }

    [Fact]
    public void IsEmail_WithNull_ReturnsFalse()
    {
        Assert.False(((string?)null).IsEmail());
    }

    [Theory]
    [InlineData("abc123def45", "12345")]
    [InlineData("no digits here", "")]
    [InlineData("", "")]
    [InlineData("v1.2.3", "123")]
    public void ExtractNumbers_KeepsOnlyDigits(string value, string expected)
    {
        Assert.Equal(expected, value.ExtractNumbers());
    }

    [Fact]
    public void ExtractNumbers_KeepsUnicodeDigits()
    {
        Assert.Equal("۱۲۳", "abc۱۲۳".ExtractNumbers());
    }

    [Fact]
    public void NormalizeWhitespace_CollapsesRunsOfWhitespaceAndTrims()
    {
        Assert.Equal("a b c", "  a\t b\n c ".NormalizeWhitespace());
    }

    [Fact]
    public void NormalizeWhitespace_WithWhitespaceOnly_ReturnsInputAsIs()
    {
        Assert.Equal("   ", "   ".NormalizeWhitespace());
    }

    [Fact]
    public void NormalizeWhitespace_WithNull_ReturnsNull()
    {
        string? value = null;

        Assert.Null(value.NormalizeWhitespace());
    }
}
