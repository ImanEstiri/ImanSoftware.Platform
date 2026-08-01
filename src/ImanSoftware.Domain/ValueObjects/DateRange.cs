// مسیر: ValueObjects/DateRange.cs
using ImanSoftware.Outcomes;

namespace ImanSoftware.Domain;

public sealed record DateRange
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public DateRange(DateTime start, DateTime end)
    {
        if (end < start)
            throw new ValidationException("End date cannot be before start date.", ["Invalid date range."]);

        Start = start.Date;
        End = end.Date;
    }

    public int Days => (int)(End - Start).TotalDays;

    public bool Contains(DateTime date) => date >= Start && date <= End;

    public bool Overlaps(DateRange other) =>
        Start <= other.End && other.Start <= End;

    public override string ToString() => $"{Start:yyyy-MM-dd} → {End:yyyy-MM-dd}";

    public static DateRange Day(DateTime date) => new(date, date);

    public static DateRange Month(int year, int month) =>
        new(start: new DateTime(year, month, 1), end: new DateTime(year, month, DateTime.DaysInMonth(year, month)));
}