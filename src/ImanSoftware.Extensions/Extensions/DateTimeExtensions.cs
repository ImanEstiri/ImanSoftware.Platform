// Core/Extensions/DateTimeExtensions.cs
namespace ImanSoftware.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Gets the start of the day (00:00:00).
    /// </summary>
    public static DateTime StartOfDay(this DateTime dateTime)
        => dateTime.Date;

    /// <summary>
    /// Gets the end of the day (23:59:59.999).
    /// </summary>
    public static DateTime EndOfDay(this DateTime dateTime)
        => dateTime.Date.AddDays(1).AddTicks(-1);

    /// <summary>
    /// Gets the start of the week (default: Monday).
    /// </summary>
    public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        int diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
        return dateTime.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// Gets the end of the week (Sunday).
    /// </summary>
    public static DateTime EndOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
        => dateTime.StartOfWeek(startOfWeek).AddDays(6).EndOfDay();

    /// <summary>
    /// Gets the number of days in the month.
    /// </summary>
    public static int DaysInMonth(this DateTime dateTime)
        => DateTime.DaysInMonth(dateTime.Year, dateTime.Month);

    /// <summary>
    /// Checks if the date is today.
    /// </summary>
    public static bool IsToday(this DateTime dateTime)
        => dateTime.Date == DateTime.Today;

    /// <summary>
    /// Checks if the date is in the future.
    /// </summary>
    public static bool IsFuture(this DateTime dateTime)
        => dateTime > DateTime.Now;

    /// <summary>
    /// Checks if the date is in the past.
    /// </summary>
    public static bool IsPast(this DateTime dateTime)
        => dateTime < DateTime.Now;

    /// <summary>
    /// Converts a DateTime to a UNIX timestamp (seconds since 1970-01-01).
    /// </summary>
    public static long ToUnixTimestamp(this DateTime dateTime)
        => new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeSeconds();

    /// <summary>
    /// Converts a UNIX timestamp to a DateTime.
    /// </summary>
    public static DateTime FromUnixTimestamp(long timestamp)
        => DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;

    /// <summary>
    /// Returns the age in years from a birthdate.
    /// </summary>
    public static int Age(this DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }
}