// مسیر: ValueObjects/Coordinates.cs
using ImanSoftware.Outcomes;

namespace ImanSoftware.Domain;

public sealed record Coordinates
{
    public double Latitude { get; }
    public double Longitude { get; }

    public Coordinates(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ValidationException($"Invalid latitude: {latitude}. Must be between -90 and 90.",
                ["Latitude out of range."]);

        if (longitude < -180 || longitude > 180)
            throw new ValidationException($"Invalid longitude: {longitude}. Must be between -180 and 180.",
                ["Longitude out of range."]);

        Latitude = Math.Round(latitude, 6);
        Longitude = Math.Round(longitude, 6);
    }

    public double DistanceTo(Coordinates other)
    {
        // فرمول هاورسین (Haversine) برای محاسبه‌ی فاصله به کیلومتر
        const double earthRadiusKm = 6371;

        var dLat = ToRadians(other.Latitude - Latitude);
        var dLon = ToRadians(other.Longitude - Longitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(other.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;

    public override string ToString() => $"{Latitude:F6}, {Longitude:F6}";
}