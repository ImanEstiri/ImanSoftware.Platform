using ImanSoftware.Framework.Abstractions.DateTimeProvider;

namespace ImanSoftware.Framework.Infrastructure.DateTimeProvider
{
    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;

        public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);

        public TimeOnly CurrentTimeOfDay => TimeOnly.FromDateTime(DateTime.Now);
    }
}
