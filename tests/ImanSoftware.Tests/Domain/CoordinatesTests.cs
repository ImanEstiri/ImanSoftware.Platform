using ImanSoftware.Domain;
using ImanSoftware.Outcomes;

namespace ImanSoftware.Tests.Domain;

public class CoordinatesTests
{
    [Fact]
    public void Ctor_WithValidValues_SetsRoundedProperties()
    {
        var coordinates = new Coordinates(12.1234567, -45.6789012);

        Assert.Equal(12.123457, coordinates.Latitude);
        Assert.Equal(-45.678901, coordinates.Longitude);
    }

    [Theory]
    [InlineData(-90, 0)]
    [InlineData(90, 0)]
    [InlineData(0, -180)]
    [InlineData(0, 180)]
    public void Ctor_AcceptsBoundaryValues(double latitude, double longitude)
    {
        var coordinates = new Coordinates(latitude, longitude);

        Assert.Equal(latitude, coordinates.Latitude);
        Assert.Equal(longitude, coordinates.Longitude);
    }

    [Theory]
    [InlineData(-90.1)]
    [InlineData(90.1)]
    public void Ctor_WithLatitudeOutOfRange_ThrowsValidationException(double latitude)
    {
        var exception = Assert.Throws<ValidationException>(() => new Coordinates(latitude, 0));

        Assert.Equal($"Invalid latitude: {latitude}. Must be between -90 and 90.", exception.Message);
        Assert.Contains("Latitude out of range.", exception.Errors);
    }

    [Theory]
    [InlineData(-180.1)]
    [InlineData(180.1)]
    public void Ctor_WithLongitudeOutOfRange_ThrowsValidationException(double longitude)
    {
        var exception = Assert.Throws<ValidationException>(() => new Coordinates(0, longitude));

        Assert.Equal($"Invalid longitude: {longitude}. Must be between -180 and 180.", exception.Message);
        Assert.Contains("Longitude out of range.", exception.Errors);
    }

    [Fact]
    public void DistanceTo_SamePoint_ReturnsZero()
    {
        var point = new Coordinates(35.6892, 51.389);

        Assert.Equal(0, point.DistanceTo(new Coordinates(35.6892, 51.389)), precision: 9);
    }

    [Fact]
    public void DistanceTo_EquatorQuarterCircumference_MatchesExpectedKilometers()
    {
        var origin = new Coordinates(0, 0);
        var quarterAround = new Coordinates(0, 90);

        // Earth radius 6371 km → quarter of a great circle ≈ 10007.54 km
        Assert.Equal(10007.54, origin.DistanceTo(quarterAround), precision: 1);
    }

    [Fact]
    public void DistanceTo_IsSymmetric()
    {
        var first = new Coordinates(35.6892, 51.389);
        var second = new Coordinates(48.8566, 2.3522);

        Assert.Equal(first.DistanceTo(second), second.DistanceTo(first), precision: 9);
    }

    [Fact]
    public void RecordEquality_UsesRoundedValues()
    {
        var first = new Coordinates(10.12345641, 20);
        var second = new Coordinates(10.12345644, 20);

        Assert.Equal(first, second);
    }

    [Fact]
    public void ToString_FormatsWithSixDecimals()
    {
        var coordinates = new Coordinates(12.1234567, -45.6789012);

        Assert.Contains("12.123457", coordinates.ToString(), StringComparison.Ordinal);
        Assert.Contains("-45.678901", coordinates.ToString(), StringComparison.Ordinal);
    }
}
