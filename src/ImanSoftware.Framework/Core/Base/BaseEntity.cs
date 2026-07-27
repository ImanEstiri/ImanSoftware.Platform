// Core/Base/BaseEntity.cs
namespace ImanSoftware.Framework.Core.Base;

/// <summary>
/// پایه‌ترین کلاس برای موجودیت‌ها با یک شناسه‌ی جنریک.
/// </summary>
/// <typeparam name="TId">نوع شناسه (مثلاً Guid, int, long)</typeparam>
public abstract class BaseEntity<TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    /// شناسه‌ی منحصربه‌فرد موجودیت.
    /// </summary>
    public TId Id { get; protected set; } = default!;

    /// <summary>
    /// سازنده‌ی پیش‌فرض (برای ORMها و Serialization).
    /// </summary>
    protected BaseEntity() { }

    /// <summary>
    /// سازنده با شناسه.
    /// </summary>
    protected BaseEntity(TId id)
    {
        Id = id;
    }

    // ===== متدهای کمکی برای مقایسه =====
    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(BaseEntity<TId>? left, BaseEntity<TId>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BaseEntity<TId>? left, BaseEntity<TId>? right)
    {
        return !Equals(left, right);
    }
}