using ImanSoftware.Domain;

namespace ImanSoftware.Tests.Domain;

public class BaseEntityTests
{
    private sealed class TestEntity : BaseEntity<Guid>
    {
        public TestEntity() { }
        public TestEntity(Guid id) : base(id) { }
    }

    private sealed class OtherEntity : BaseEntity<Guid>
    {
        public OtherEntity(Guid id) : base(id) { }
    }

    private sealed class IntEntity : BaseEntity<int>
    {
        public IntEntity(int id) : base(id) { }
    }

    [Fact]
    public void Ctor_WithId_SetsId()
    {
        var id = Guid.NewGuid();

        var entity = new TestEntity(id);

        Assert.Equal(id, entity.Id);
    }

    [Fact]
    public void Equals_WithSameIdAndType_ReturnsTrue()
    {
        var id = Guid.NewGuid();

        Assert.Equal(new TestEntity(id), new TestEntity(id));
    }

    [Fact]
    public void Equals_WithDifferentId_ReturnsFalse()
    {
        var entity = new TestEntity(Guid.NewGuid());

        Assert.NotEqual(entity, new TestEntity(Guid.NewGuid()));
    }

    [Fact]
    public void Equals_WithDifferentRuntimeType_ReturnsFalse()
    {
        var id = Guid.NewGuid();

        Assert.False(new TestEntity(id).Equals(new OtherEntity(id)));
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        Assert.False(new TestEntity(Guid.NewGuid()).Equals(null));
    }

    [Fact]
    public void Equals_WithSameReference_ReturnsTrue()
    {
        var entity = new TestEntity(Guid.NewGuid());

        Assert.True(entity.Equals((object)entity));
    }

    [Fact]
    public void Equals_TwoTransientEntitiesWithDefaultId_AreEqual()
    {
        Assert.Equal(new TestEntity(), new TestEntity());
    }

    [Fact]
    public void EqualityOperators_AreNullSafe()
    {
        TestEntity? nullEntity = null;

        Assert.True(nullEntity == null);
        Assert.False(nullEntity == new TestEntity(Guid.NewGuid()));
        Assert.True(nullEntity != new TestEntity(Guid.NewGuid()));
        Assert.False(nullEntity != null);
    }

    [Fact]
    public void EqualityOperator_ComparesById()
    {
        var id = Guid.NewGuid();

        Assert.True(new TestEntity(id) == new TestEntity(id));
        Assert.True(new TestEntity(id) != new TestEntity(Guid.NewGuid()));
    }

    [Fact]
    public void GetHashCode_IsDerivedFromId()
    {
        var id = Guid.NewGuid();

        Assert.Equal(id.GetHashCode(), new TestEntity(id).GetHashCode());
    }

    [Fact]
    public void IntKeyedEntity_ComparesById()
    {
        Assert.Equal(new IntEntity(7), new IntEntity(7));
        Assert.NotEqual(new IntEntity(7), new IntEntity(8));
    }
}
