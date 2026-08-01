// مسیر: ValueObjects/EmailAddress.cs
using System.Text.RegularExpressions;

namespace ImanSoftware.Domain;

public sealed record EmailAddress
{
    public string Value { get; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new Outcomes.ValidationException("Email address cannot be null or empty.", ["Email is required."]);

        if (!IsValid(value))
            throw new Outcomes.ValidationException($"'{value}' is not a valid email address.", ["Invalid email format."]);

        Value = value;
    }

    private static bool IsValid(string email)
    {
        // یک Regex ساده و معتبر برای ایمیل
        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return regex.IsMatch(email);
    }

    public override string ToString() => Value;

    public static implicit operator string(EmailAddress email) => email.Value;
    public static explicit operator EmailAddress(string value) => new(value);
}