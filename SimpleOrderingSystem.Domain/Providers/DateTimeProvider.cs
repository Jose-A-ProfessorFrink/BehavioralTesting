
namespace SimpleOrderingSystem.Domain.Providers;

internal class DateTimeProvider:IDateTimeProvider
{
    public DateTime UtcNow() => DateTime.UtcNow;
}