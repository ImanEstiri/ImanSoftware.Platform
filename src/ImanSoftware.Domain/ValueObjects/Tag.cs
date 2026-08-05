using ImanSoftware.Outcomes;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ImanSoftware.Domain;

/// <summary>
/// Value Object برای برچسب (Tag) که به یک موجودیت والد متصل است.
/// </summary>
public sealed record Tag<TParentId> where TParentId : IEquatable<TParentId>
{
    public TParentId ParentId { get; }
    public string Name { get; }
    public TagType TagType { get; }
    public string Slug => GenerateSlug(Name);
    public string Color => GetColor(TagType);

    public Tag(TParentId parentId, string name, TagType tagType = TagType.General)
    {
        // اعتبارسنجی ParentId
        if (parentId is null || parentId.Equals(default(TParentId)))
            throw new ValidationException("ParentId cannot be null or default.", ["ParentId is required."]);

        // اعتبارسنجی Name
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Tag name cannot be null or empty.", ["Tag name is required."]);

        if (name.Length > 50)
            throw new ValidationException("Tag name cannot exceed 50 characters.", ["Tag name is too long."]);

        ParentId = parentId;
        Name = name.Trim();
        TagType = tagType;
    }


    private static string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");    // حذف کاراکترهای غیرمجاز
        slug = Regex.Replace(slug, @"\s+", "-");            // تبدیل فاصله به خط تیره
        slug = Regex.Replace(slug, @"-+", "-");             // حذف خط تیره‌های تکراری
        return slug.Trim('-');
    }

    private static string GetColor(TagType tagType)
    {
        var field = tagType.GetType().GetField(tagType.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? "#6c757d"; // پیش‌فرض
    }

    public override string ToString() => $"{Name} ({Slug})";
}
