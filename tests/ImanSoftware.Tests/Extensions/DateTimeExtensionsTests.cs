using ImanSoftware.Extensions;

namespace ImanSoftware.Tests.Extensions;

public class DateTimeExtensionsTests
{
    [Fact]
    public void StartOfDay_ReturnsDateAtMidnight()
    {
        var result = new DateTime(2024, 5, 15, 10, 30, 45).StartOfDay();

        Assert.Equal(new DateTime(2024, 5, 15), result);
    }

    [Fact]
    public void EndOfDay_ReturnsLastTickOfDay()
    {
        var result = new DateTime(2024, 5, 15, 10, 30, 45).EndOfDay();

        Assert.Equal(new DateTime(2024, 5, 15, 23, 59, 59, 999).AddTicks(9999), result);
    }

    [Fact]
    public void StartOfWeek_DefaultMondayStart_ReturnsMonday()
    {
        // 2024-05-15 was a Wednesday.
        var result = new DateTime(2024, 5, 15).StartOfWeek();

        Assert.Equal(new DateTime(2024, 5, 13), result);
    }

    [Fact]
    public void StartOfWeek_SundayStart_ReturnsSunday()
    {
        var result = new DateTime(2024, 5, 15).StartOfWeek(DayOfWeek.Sunday);

        Assert.Equal(new DateTime(2024, 5, 12), result);
    }

    [Fact]
    public void StartOfWeek_OnSundayWithSundayStart_ReturnsSameDay()
    {
        var result = new DateTime(2024, 5, 19).StartOfWeek(DayOfWeek.Sunday);

        Assert.Equal(new DateTime(2024, 5, 19), result);
    }

    [Fact]
    public void EndOfWeek_DefaultMondayStart_ReturnsSundayEndOfDay()
    {
        var result = new DateTime(2024, 5, 15).EndOfWeek();

        Assert.Equal(new DateTime(2024, 5, 19, 23, 59, 59, 999).AddTicks(9999), result);
    }

    [Theory]
    [InlineData("2024-02-15", 29)]
    [InlineData("2023-02-15", 28)]
    [InlineData("2024-04-01", 30)]
    public void DaysInMonth_ReturnsDaysOfMonth(string date, int expected)
    {
        Assert.Equal(expected, DateTime.Parse(date).DaysInMonth());
    }

    [Fact]
    public void ToUnixTimestamp_WithUtcEpoch_ReturnsZero()
    {
        Assert.Equal(0, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUnixTimestamp());
    }

    [Fact]
    public void ToUnixTimestamp_WithKnownUtcDate_ReturnsExpectedSeconds()
    {
        Assert.Equal(946684800, new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUnixTimestamp());
    }

    [Fact]
    public void FromUnixTimestamp_ReturnsUtcDateTime()
    {
        var result = DateTimeExtensions.FromUnixTimestamp(946684800);

        Assert.Equal(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), result);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }

    [Fact]
    public void UnixTimestampConversions_RoundTrip()
    {
        // Truncate to whole seconds — Unix timestamps have second resolution.
        var utcNowValue = DateTime.UtcNow;
        var utcNow = new DateTime(
            utcNowValue.Ticks - (utcNowValue.Ticks % TimeSpan.TicksPerSecond),
            DateTimeKind.Utc);

        var roundTripped = DateTimeExtensions.FromUnixTimestamp(utcNow.ToUnixTimestamp());

        Assert.Equal(utcNow, roundTripped);
    }

    [Fact]
    public void Age_ForBirthdayEarlierThisYear_ReturnsExactYears()
    {
        Assert.Equal(30, DateTime.Today.AddYears(-30).Age());
    }

    [Fact]
    public void Age_ForBirthdayLaterThisYear_ReturnsYearsMinusOne()
    {
        Assert.Equal(29, DateTime.Today.AddYears(-30).AddDays(1).Age());
    }

    [Fact]
    public void IsToday_ForToday_ReturnsTrue()
    {
        Assert.True(DateTime.Today.IsToday());
    }

    [Fact]
    public void IsToday_ForYesterday_ReturnsFalse()
    {
        Assert.False(DateTime.Today.AddDays(-1).IsToday());
    }

    [Fact]
    public void IsFuture_ForOneHourAhead_ReturnsTrue()
    {
        Assert.True(DateTime.Now.AddHours(1).IsFuture());
    }

    [Fact]
    public void IsFuture_ForOneHourAgo_ReturnsFalse()
    {
        Assert.False(DateTime.Now.AddHours(-1).IsFuture());
    }

    [Fact]
    public void IsPast_ForOneHourAgo_ReturnsTrue()
    {
        Assert.True(DateTime.Now.AddHours(-1).IsPast());
    }

    [Fact]
    public void ToPersianDateString_ForNowruz1403_ReturnsFirstDayOfYear()
    {
        DateTime? date = new DateTime(2024, 3, 20);

        Assert.Equal("1403/01/01", date.ToPersianDateString());
    }

    [Fact]
    public void ToPersianDateString_WithCustomSeparator_UsesSeparator()
    {
        DateTime? date = new DateTime(2024, 3, 20);

        Assert.Equal("1403-01-01", date.ToPersianDateString("-"));
    }

    [Fact]
    public void ToPersianDateString_WithNull_ReturnsDefaultValue()
    {
        DateTime? date = null;

        Assert.Equal(string.Empty, date.ToPersianDateString());
        Assert.Equal("N/A", date.ToPersianDateString(defaultValue: "N/A"));
    }

    [Fact]
    public void ToGregorianDateTime_ForNowruz1403_ReturnsGregorianDate()
    {
        Assert.Equal(new DateTime(2024, 3, 20), "1403/01/01".ToGregorianDateTime());
    }

    [Fact]
    public void ToGregorianDateTime_WithSingleDigitParts_Parses()
    {
        Assert.Equal(new DateTime(2024, 3, 20), "1403/1/1".ToGregorianDateTime());
    }

    [Theory]
    [InlineData("1403/13/01")]
    [InlineData("1403/01/32")]
    [InlineData("abc/01/01")]
    [InlineData("1403-01")]
    [InlineData("")]
    public void ToGregorianDateTime_WithInvalidInput_ReturnsNull(string persianDate)
    {
        Assert.Null(persianDate.ToGregorianDateTime());
    }

    [Fact]
    public void GetPersianMonthName_ForFirstOfFarvardin_ReturnsFarvardin()
    {
        DateTime? date = new DateTime(2024, 3, 20);

        Assert.Equal("فروردین", date.GetPersianMonthName());
    }

    [Fact]
    public void GetPersianMonthName_ForFirstOfTir_ReturnsTir()
    {
        DateTime? date = new DateTime(2024, 6, 21);

        Assert.Equal("تیر", date.GetPersianMonthName());
    }

    [Fact]
    public void GetPersianMonthName_WithNull_ReturnsEmpty()
    {
        DateTime? date = null;

        Assert.Equal(string.Empty, date.GetPersianMonthName());
    }
}
