
namespace ImanSoftware.Domain;

/// <summary>
/// کلاس پایه برای موجودیت‌هایی که می‌توانند برچسب (Tag) داشته باشند.
/// </summary>
public abstract class TaggedEntity<TId> : BaseEntity<TId> where TId : IEquatable<TId>
{
    private readonly List<Tag<TId>> _tags = new();

    /// <summary>
    /// مجموعه‌ی برچسب‌های موجودیت (فقط خواندنی).
    /// </summary>
    public IReadOnlyCollection<Tag<TId>> Tags => _tags.AsReadOnly();

    protected TaggedEntity() { }
    protected TaggedEntity(TId id) : base(id) { }

    /// <summary>
    /// یک برچسب جدید به موجودیت اضافه می‌کند.
    /// </summary>
    public void AddTag(string tagName , TagType tagType)
    {
        var tag = new Tag<TId>(Id , tagName , tagType);

        // جلوگیری از اضافه شدن تگ تکراری (با ParentId و Name یکسان)
        if (_tags.Any(t => t.Name.Equals(tag.Name, StringComparison.OrdinalIgnoreCase)))
            return;

        _tags.Add(tag);
    }

    /// <summary>
    /// یک برچسب را با استفاده از اسم و ParentId حذف می‌کند.
    /// </summary>
    public bool RemoveTag(string name)
    {
        var tag = _tags.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return tag is not null && _tags.Remove(tag);
    }

    /// <summary>
    /// همه‌ی برچسب‌ها را پاک می‌کند.
    /// </summary>
    public void ClearTags() => _tags.Clear();
}