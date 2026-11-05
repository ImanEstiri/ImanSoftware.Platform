using ImanSoftware.Domain;
using ImanSoftware.Outcomes;

namespace ImanSoftware.Tests.Domain;

public class DateRangeTests
{
    [Fact]
    public void Ctor_TruncatesTimeComponentsToDate()
    {
        var range = new DateRange(
            new DateTime(2024, 1, 1, 8, 30, 0),
            new DateTime(2024, 1, 5, 22, 0, 0));

        Assert.Equal(new DateTime(2024, 1, 1), range.Start);
        Assert.Equal(new DateTime(2024, 1, 5), range.End);
    }

    [Fact]
    public void Ctor_WithEndBeforeStart_ThrowsValidationException()
    {
        var exception = Assert.Throws<ValidationException>(() => new DateRange(
            new DateTime(2024, 1, 5),
            new DateTime(2024, 1, 1)));

        Assert.Equal("End date cannot be before start date.", exception.Message);
        Assert.Contains("Invalid date range.", exception.Errors);
    }

    [Fact]
    public void Ctor_WithSameDayButReversedTimes_ThrowsValidationException()
    {
        // Truncation happens after validation, so a later time on the same day as an earlier end still throws.
        Assert.Throws<ValidationException>(() => new DateRange(
            new DateTime(2024, 1, 2, 23, 0, 0),
            new DateTime(2024, 1, 2, 0, 0, 0)));
    }

    [Fact]
    public void Days_SingleDayRange_IsZero()
    {
        Assert.Equal(0, new DateRange(new(2024, 1, 1), new(2024, 1, 1)).Days);
    }

    [Fact]
    public void Days_FiveDayRange_IsFour()
    {
        Assert.Equal(4, new DateRange(new(2024, 1, 1), new(2024, 1, 5)).Days);
    }

    [Fact]
    public void Contains_IsInclusiveOnBothBounds()
    {
        var range = new DateRange(new(2024, 1, 1), new(2024, 1, 5));

        Assert.True(range.Contains(new DateTime(2024, 1, 1)));
        Assert.True(range.Contains(new DateTime(2024, 1, 5)));
        Assert.True(range.Contains(new DateTime(2024, 1, 3)));
        Assert.False(range.Contains(new DateTime(2024, 1, 6)));
        Assert.False(range.Contains(new DateTime(2023, 12, 31)));
    }

    [Fact]
    public void Contains_TimeOfDayOnEndDay_IsNotContained()
    {
        var range = new DateRange(new(2024, 1, 1), new(2024, 1, 5));

        Assert.False(range.Contains(new DateTime(2024, 1, 5, 15, 0, 0)));
    }

    [Fact]
    public void Overlaps_TouchingRanges_CountAsOverlapping()
    {
        var first = new DateRange(new(2024, 1, 1), new(2024, 1, 5));
        var second = new DateRange(new(2024, 1, 5), new(2024, 1, 10));

        Assert.True(first.Overlaps(second));
        Assert.True(second.Overlaps(first));
    }

    [Fact]
    public void Overlaps_DisjointRanges_ReturnsFalse()
    {
        var first = new DateRange(new(2024, 1, 1), new(2024, 1, 5));
        var second = new DateRange(new(2024, 2, 1), new(2024, 2, 10));

        Assert.False(first.Overlaps(second));
    }

    [Fact]
    public void Overlaps_ContainingRange_ReturnsTrue()
    {
        var outer = new DateRange(new(2024, 1, 1), new(2024, 1, 31));
        var inner = new DateRange(new(2024, 1, 10), new(2024, 1, 20));

        Assert.True(outer.Overlaps(inner));
    }

    [Fact]
    public void Day_ReturnsSingleDayRange()
    {
        var range = DateRange.Day(new DateTime(2024, 3, 15, 10, 0, 0));

        Assert.Equal(new DateTime(2024, 3, 15), range.Start);
        Assert.Equal(new DateTime(2024, 3, 15), range.End);
        Assert.Equal(0, range.Days);
    }

    [Fact]
    public void Month_ReturnsFirstToLastDay()
    {
        var range = DateRange.Month(2024, 2);

        Assert.Equal(new DateTime(2024, 2, 1), range.Start);
        Assert.Equal(new DateTime(2024, 2, 29), range.End);
        Assert.Equal(28, range.Days);
    }

    [Fact]
    public void Month_NonLeapFebruary_Has28Days()
    {
        Assert.Equal(27, DateRange.Month(2023, 2).Days);
    }

    [Fact]
    public void Month_WithInvalidMonth_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => DateRange.Month(2024, 13));
    }

    [Fact]
    public void ToString_UsesArrowFormat()
    {
        var range = new DateRange(new(2024, 1, 1), new(2024, 1, 5));

        Assert.Equal("2024-01-01 → 2024-01-05", range.ToString());
    }
}
