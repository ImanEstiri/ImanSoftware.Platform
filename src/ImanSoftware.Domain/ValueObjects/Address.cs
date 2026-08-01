// مسیر: ValueObjects/Address.cs
using ImanSoftware.Outcomes;

namespace ImanSoftware.Domain;

public sealed record Address
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }
    public string Country { get; }

    public Address(string street, string city, string state, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ValidationException("Street cannot be null or empty.", ["Street is required."]);

        if (string.IsNullOrWhiteSpace(city))
            throw new ValidationException("City cannot be null or empty.", ["City is required."]);

        if (string.IsNullOrWhiteSpace(country))
            throw new ValidationException("Country cannot be null or empty.", ["Country is required."]);

        Street = street;
        City = city;
        State = state ?? string.Empty;
        PostalCode = postalCode ?? string.Empty;
        Country = country;
    }

    public override string ToString()
    {
        var parts = new[] { Street, City, State, PostalCode, Country };
        return string.Join(", ", parts.Where(p => !string.IsNullOrEmpty(p)));
    }
}