
namespace SimpleOrderingSystem.Domain.Providers;

internal class GuidProvider: IGuidProvider
{
    public Guid NewGuid() => Guid.NewGuid();
}