

namespace ImanSoftware.Framework.Abstractions.DateTimeProvider
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }

        DateOnly Today { get; }

        TimeOnly CurrentTimeOfDay { get; }

    }
}
