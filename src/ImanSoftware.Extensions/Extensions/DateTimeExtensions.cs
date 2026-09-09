// Core/Extensions/DateTimeExtensions.cs
using System.Globalization;

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

    private static readonly PersianCalendar PersianCalendar = new();

    /// <summary>
    /// Converts a DateTime? to a Persian (Solar Hijri) date string.
    /// Returns defaultValue if the input is null.
    /// </summary>
    /// <param name="dateTime">The nullable DateTime to convert.</param>
    /// <param name="separator">Separator between year, month, and day (default: "/").</param>
    /// <param name="defaultValue">Value to return if DateTime is null (default: string.Empty).</param>
    public static string ToPersianDateString(
        this DateTime? dateTime,
        string separator = "/",
        string defaultValue = "")
    {
        if (!dateTime.HasValue)
            return defaultValue;

        int year = PersianCalendar.GetYear(dateTime.Value);
        int month = PersianCalendar.GetMonth(dateTime.Value);
        int day = PersianCalendar.GetDayOfMonth(dateTime.Value);
        return $"{year}{separator}{month:D2}{separator}{day:D2}";
    }

    /// <summary>
    /// Converts a Persian date string (yyyy/MM/dd) to DateTime.
    /// </summary>
    public static DateTime? ToGregorianDateTime(this string persianDate, string separator = "/")
    {
        var parts = persianDate.Split(separator);
        if (parts.Length != 3)
            return null;

        if (!int.TryParse(parts[0], out int year) ||
            !int.TryParse(parts[1], out int month) ||
            !int.TryParse(parts[2], out int day))
            return null;

        try
        {
            return PersianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the Persian month name.
    /// </summary>
    public static string GetPersianMonthName(this DateTime? dateTime)
    {
        if (!dateTime.HasValue)
            return string.Empty; // یا "نامشخص"

        int month = PersianCalendar.GetMonth(dateTime.Value);
        return month switch
        {
            1 => "فروردین",
            2 => "اردیبهشت",
            3 => "خرداد",
            4 => "تیر",
            5 => "مرداد",
            6 => "شهریور",
            7 => "مهر",
            8 => "آبان",
            9 => "آذر",
            10 => "دی",
            11 => "بهمن",
            12 => "اسفند",
            _ => string.Empty
        };
    }
}
