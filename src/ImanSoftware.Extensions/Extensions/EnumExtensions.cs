// Core/Extensions/EnumExtensions.cs
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ImanSoftware.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Gets the Description attribute value of an enum value.
    /// </summary>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }

    /// <summary>
    /// Gets the Display attribute value of an enum value.
    /// </summary>
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DisplayAttribute>();
        return attribute?.Name ?? value.ToString();
    }

    /// <summary>
    /// Checks if the enum value is defined in the enum.
    /// </summary>
    public static bool IsDefined<TEnum>(this TEnum value) where TEnum : struct, Enum
        => Enum.IsDefined(typeof(TEnum), value);

    /// <summary>
    /// Gets all values of an enum type.
    /// </summary>
    public static IEnumerable<TEnum> GetValues<TEnum>() where TEnum : struct, Enum
        => Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

    /// <summary>
    /// Converts an enum to its underlying integral value.
    /// </summary>
    public static int ToInt(this Enum value)
        => Convert.ToInt32(value);

    /// <summary>
    /// Parses a string to an enum value (case-insensitive).
    /// </summary>
    public static TEnum Parse<TEnum>(string value) where TEnum : struct, Enum
        => (TEnum)Enum.Parse(typeof(TEnum), value, true);
}