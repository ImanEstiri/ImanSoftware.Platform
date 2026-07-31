
// Core/Extensions/StringExtensions.cs
using System.Text.RegularExpressions;

namespace ImanSoftware.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Checks if a string is null, empty, or whitespace.
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value)
        => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Checks if a string is null or empty.
    /// </summary>
    public static bool IsNullOrEmpty(this string? value)
        => string.IsNullOrEmpty(value);

    /// <summary>
    /// Truncates a string to a maximum length with an optional suffix.
    /// </summary>
    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..(maxLength - suffix.Length)] + suffix;
    }

    /// <summary>
    /// Converts a string to camelCase.
    /// </summary>
    public static string ToCamelCase(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    /// <summary>
    /// Converts a string to snake_case.
    /// </summary>
    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return Regex.Replace(value, "([a-z0-9])([A-Z])", "$1_$2").ToLowerInvariant();
    }

    /// <summary>
    /// Converts a string to kebab-case.
    /// </summary>
    public static string ToKebabCase(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return Regex.Replace(value, "([a-z0-9])([A-Z])", "$1-$2").ToLowerInvariant();
    }

    /// <summary>
    /// Checks if a string is a valid email address.
    /// </summary>
    public static bool IsEmail(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        try
        {
            var addr = new System.Net.Mail.MailAddress(value);
            return addr.Address == value;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Extracts numbers from a string.
    /// </summary>
    public static string ExtractNumbers(this string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return new string(value.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Replaces multiple spaces with a single space.
    /// </summary>
    public static string NormalizeWhitespace(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return value;
        return Regex.Replace(value, @"\s+", " ").Trim();
    }
}