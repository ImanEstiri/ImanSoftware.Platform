using ImanSoftware.Domain;
using ImanSoftware.Outcomes;

namespace ImanSoftware.Tests.Domain;

public class TagTests
{
    private static readonly Guid ParentId = Guid.NewGuid();

    [Fact]
    public void Ctor_WithValidData_SetsProperties()
    {
        var tag = new Tag<Guid>(ParentId, "Science", TagType.Science);

        Assert.Equal(ParentId, tag.ParentId);
        Assert.Equal("Science", tag.Name);
        Assert.Equal(TagType.Science, tag.TagType);
    }

    [Fact]
    public void Ctor_WithoutTagType_DefaultsToGeneral()
    {
        var tag = new Tag<Guid>(ParentId, "General");

        Assert.Equal(TagType.General, tag.TagType);
    }

    [Fact]
    public void Ctor_WithDefaultParentId_ThrowsValidationException()
    {
        var exception = Assert.Throws<ValidationException>(() => new Tag<Guid>(Guid.Empty, "name"));

        Assert.Equal("ParentId cannot be null or default.", exception.Message);
        Assert.Contains("ParentId is required.", exception.Errors);
    }

    [Fact]
    public void Ctor_WithNullName_ThrowsValidationException()
    {
        var exception = Assert.Throws<ValidationException>(() => new Tag<Guid>(ParentId, null!));

        Assert.Equal("Tag name cannot be null or empty.", exception.Message);
    }

    [Fact]
    public void Ctor_WithWhitespaceName_ThrowsValidationException()
    {
        Assert.Throws<ValidationException>(() => new Tag<Guid>(ParentId, "   "));
    }

    [Fact]
    public void Ctor_WithNameLongerThan50Chars_ThrowsValidationException()
    {
        var longName = new string('a', 51);

        var exception = Assert.Throws<ValidationException>(() => new Tag<Guid>(ParentId, longName));

        Assert.Equal("Tag name cannot exceed 50 characters.", exception.Message);
    }

    [Fact]
    public void Ctor_TrimsName()
    {
        var tag = new Tag<Guid>(ParentId, "  trimmed name  ");

        Assert.Equal("trimmed name", tag.Name);
    }

    [Theory]
    [InlineData("Hello World!", "hello-world")]
    [InlineData("C# & .NET", "c-net")]
    [InlineData("  Multiple   Spaces  ", "multiple-spaces")]
    [InlineData("---dashes---", "dashes")]
    [InlineData("ALREADY lower", "already-lower")]
    public void Slug_NormalizesName(string name, string expected)
    {
        var tag = new Tag<Guid>(ParentId, name);

        Assert.Equal(expected, tag.Slug);
    }

    [Theory]
    [InlineData(TagType.Technology, "#007bff")]
    [InlineData(TagType.Science, "#28a745")]
    [InlineData(TagType.Art, "#dc3545")]
    [InlineData(TagType.Business, "#ffc107")]
    [InlineData(TagType.Health, "#17a2b8")]
    [InlineData(TagType.Education, "#6f42c1")]
    [InlineData(TagType.General, "#6c757d")]
    public void Color_MapsTagTypeToDescriptionColor(TagType tagType, string expected)
    {
        var tag = new Tag<Guid>(ParentId, "name", tagType);

        Assert.Equal(expected, tag.Color);
    }

    [Fact]
    public void ToString_ReturnsNameAndSlug()
    {
        var tag = new Tag<Guid>(ParentId, "Hello World!");

        Assert.Equal("Hello World! (hello-world)", tag.ToString());
    }

    [Fact]
    public void RecordEquality_ComparesParentIdNameAndTagType()
    {
        var first = new Tag<Guid>(ParentId, "same", TagType.Art);
        var second = new Tag<Guid>(ParentId, "same", TagType.Art);

        Assert.Equal(first, second);
        Assert.NotEqual(first, new Tag<Guid>(ParentId, "same", TagType.Science));
        Assert.NotEqual(first, new Tag<Guid>(ParentId, "other", TagType.Art));
        Assert.NotEqual(first, new Tag<Guid>(Guid.NewGuid(), "same", TagType.Art));
    }
}
