using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ImanSoftware.Extensions;

namespace ImanSoftware.Tests.Extensions;

public class EnumExtensionsTests
{
    private enum SampleEnum
    {
        [Description("First Description")] First = 1,
        [Display(Name = "Display Name")] Second = 2,
        Third = 3,
        Multi_Word_Value = 4
    }

    [Fact]
    public void GetDescription_WithDescriptionAttribute_ReturnsDescription()
    {
        Assert.Equal("First Description", SampleEnum.First.GetDescription());
    }

    [Fact]
    public void GetDescription_WithoutDescriptionAttribute_FallsBackToName()
    {
        Assert.Equal("Third", SampleEnum.Third.GetDescription());
    }

    [Fact]
    public void GetDisplayName_WithDisplayAttribute_ReturnsName()
    {
        Assert.Equal("Display Name", SampleEnum.Second.GetDisplayName());
    }

    [Fact]
    public void GetDisplayName_WithoutDisplayAttribute_FallsBackToName()
    {
        Assert.Equal("First", SampleEnum.First.GetDisplayName());
    }

    [Fact]
    public void IsDefined_ForDefinedValue_ReturnsTrue()
    {
        Assert.True(SampleEnum.First.IsDefined());
    }

    [Fact]
    public void IsDefined_ForUndefinedValue_ReturnsFalse()
    {
        Assert.False(((SampleEnum)99).IsDefined());
    }

    [Fact]
    public void GetValues_ReturnsAllEnumValues()
    {
        var values = EnumExtensions.GetValues<SampleEnum>().ToArray();

        Assert.Equal(4, values.Length);
        Assert.Contains(SampleEnum.First, values);
        Assert.Contains(SampleEnum.Multi_Word_Value, values);
    }

    [Fact]
    public void ToInt_ReturnsUnderlyingValue()
    {
        Assert.Equal(2, SampleEnum.Second.ToInt());
    }

    [Fact]
    public void Parse_IgnoresCase()
    {
        Assert.Equal(SampleEnum.Second, EnumExtensions.Parse<SampleEnum>("SECOND"));
    }

    [Fact]
    public void Parse_WithInvalidName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => EnumExtensions.Parse<SampleEnum>("Unknown"));
    }

    [Fact]
    public void Parse_WithNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => EnumExtensions.Parse<SampleEnum>(null!));
    }

    [Fact]
    public void EnumToString_ReplacesUnderscoresWithSpaces()
    {
        Assert.Equal("Multi Word Value", SampleEnum.Multi_Word_Value.EnumToString());
    }

    [Fact]
    public void EnumToString_WithNull_ReturnsEmpty()
    {
        SampleEnum? value = null;

        Assert.Equal(string.Empty, value.EnumToString());
    }
}
