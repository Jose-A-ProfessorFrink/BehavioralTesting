
namespace SimpleOrderingSystem.Domain.Providers;

internal class DateTimeProvider:IDateTimeProvider
{
    public DateTime NowUtc() => DateTime.UtcNow;
}