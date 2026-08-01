// مسیر: ValueObjects/PhoneNumber.cs
using System.Text.RegularExpressions;
using ImanSoftware.Outcomes;

namespace ImanSoftware.Domain;

public sealed record PhoneNumber
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException("Phone number cannot be null or empty.", ["Phone is required."]);

        if (!IsValid(value))
            throw new ValidationException($"'{value}' is not a valid phone number.", ["Invalid phone format."]);

        Value = value;
    }

    private static bool IsValid(string phone)
    {
        // پشتیبانی از شماره‌های بین‌المللی با + و بدون صفر ابتدایی
        var regex = new Regex(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return regex.IsMatch(phone);
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phone) => phone.Value;
    public static explicit operator PhoneNumber(string value) => new(value);
}