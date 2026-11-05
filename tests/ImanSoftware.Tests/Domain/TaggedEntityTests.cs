using ImanSoftware.Domain;
using ImanSoftware.Outcomes;

namespace ImanSoftware.Tests.Domain;

public class TaggedEntityTests
{
    private sealed class TestTaggedEntity : TaggedEntity<Guid>
    {
        public TestTaggedEntity() { }
        public TestTaggedEntity(Guid id) : base(id) { }
    }

    private static TestTaggedEntity CreateEntity() => new(Guid.NewGuid());

    [Fact]
    public void AddTag_WithValidName_AddsTag()
    {
        var entity = CreateEntity();

        entity.AddTag("technology", TagType.Technology);

        var tag = Assert.Single(entity.Tags);
        Assert.Equal("technology", tag.Name);
        Assert.Equal(TagType.Technology, tag.TagType);
        Assert.Equal(entity.Id, tag.ParentId);
    }

    [Fact]
    public void AddTag_OnEntityWithDefaultId_ThrowsValidationException()
    {
        var entity = new TestTaggedEntity();

        var exception = Assert.Throws<ValidationException>(() => entity.AddTag("name", TagType.General));

        Assert.Equal("ParentId cannot be null or default.", exception.Message);
    }

    [Fact]
    public void AddTag_DuplicateName_IgnoresSilently()
    {
        var entity = CreateEntity();

        entity.AddTag("science", TagType.Science);
        entity.AddTag("science", TagType.Art);

        Assert.Single(entity.Tags);
    }

    [Fact]
    public void AddTag_DuplicateNameDifferentCase_IgnoresSilently()
    {
        var entity = CreateEntity();

        entity.AddTag("Business", TagType.Business);
        entity.AddTag("BUSINESS", TagType.Health);

        Assert.Single(entity.Tags);
    }

    [Fact]
    public void AddTag_FirstTagTypeWinsOnDuplicate()
    {
        var entity = CreateEntity();

        entity.AddTag("art", TagType.Art);
        entity.AddTag("ART", TagType.Science);

        Assert.Equal(TagType.Art, entity.Tags.Single().TagType);
    }

    [Fact]
    public void AddTag_TrimsNameBeforeStoring()
    {
        var entity = CreateEntity();

        entity.AddTag("  health  ", TagType.Health);

        Assert.Equal("health", entity.Tags.Single().Name);
    }

    [Fact]
    public void RemoveTag_ExistingNameDifferentCase_RemovesAndReturnsTrue()
    {
        var entity = CreateEntity();
        entity.AddTag("education", TagType.Education);

        var removed = entity.RemoveTag("EDUCATION");

        Assert.True(removed);
        Assert.Empty(entity.Tags);
    }

    [Fact]
    public void RemoveTag_MissingName_ReturnsFalse()
    {
        var entity = CreateEntity();
        entity.AddTag("education", TagType.Education);

        Assert.False(entity.RemoveTag("missing"));
    }

    [Fact]
    public void ClearTags_RemovesAllTags()
    {
        var entity = CreateEntity();
        entity.AddTag("a", TagType.General);
        entity.AddTag("b", TagType.General);

        entity.ClearTags();

        Assert.Empty(entity.Tags);
    }

    [Fact]
    public void Tags_IsLiveViewOverInternalCollection()
    {
        var entity = CreateEntity();

        entity.AddTag("a", TagType.General);
        var beforeCount = entity.Tags.Count;
        entity.AddTag("b", TagType.General);

        Assert.Equal(1, beforeCount);
        Assert.Equal(2, entity.Tags.Count);
    }
}
