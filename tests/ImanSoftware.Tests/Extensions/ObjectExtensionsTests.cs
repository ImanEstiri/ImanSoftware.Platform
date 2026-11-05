using ImanSoftware.Extensions;

namespace ImanSoftware.Tests.Extensions;

public class ObjectExtensionsTests
{
    private sealed class SampleDto
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
        public string? Optional { get; set; }
    }

    [Fact]
    public void ToJson_UsesCamelCasePropertyNames()
    {
        var dto = new SampleDto { Name = "x", Value = 5 };

        Assert.Equal("""{"name":"x","value":5,"optional":null}""", dto.ToJson());
    }

    [Fact]
    public void ToJson_Indented_WritesFormattedOutput()
    {
        var dto = new SampleDto { Name = "x", Value = 5 };

        var json = dto.ToJson(indented: true);

        Assert.Contains(Environment.NewLine, json, StringComparison.Ordinal);
    }

    [Fact]
    public void IsNull_ForNullReference_ReturnsTrue()
    {
        object? value = null;

        Assert.True(value.IsNull());
    }

    [Fact]
    public void IsNull_ForNonNullReference_ReturnsFalse()
    {
        Assert.False("".IsNull());
    }

    [Fact]
    public void IsNull_ForBoxedValueType_ReturnsFalse()
    {
        object? boxed = 0;

        Assert.False(boxed.IsNull());
    }

    [Fact]
    public void ThrowIfNull_WithNull_ThrowsArgumentNullExceptionWithParamName()
    {
        string? value = null;

        var exception = Assert.Throws<ArgumentNullException>(() => value.ThrowIfNull("value"));

        Assert.Equal("value", exception.ParamName);
    }

    [Fact]
    public void ThrowIfNull_WithNonNull_ReturnsSameInstance()
    {
        var value = "data";

        Assert.Same(value, value.ThrowIfNull("value"));
    }

    [Fact]
    public void DeepCopy_ProducesEqualButDistinctInstance()
    {
        var dto = new SampleDto { Name = "original", Value = 7 };

        var copy = dto.DeepCopy();

        Assert.NotSame(dto, copy);
        Assert.Equal(dto.Name, copy!.Name);
        Assert.Equal(dto.Value, copy.Value);
    }

    [Fact]
    public void DeepCopy_MutationOfCopyDoesNotAffectOriginal()
    {
        var dto = new SampleDto { Name = "original", Value = 7 };

        var copy = dto.DeepCopy()!;
        copy.Name = "changed";

        Assert.Equal("original", dto.Name);
        Assert.Equal("changed", copy.Name);
    }
}
