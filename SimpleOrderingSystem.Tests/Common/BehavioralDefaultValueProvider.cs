namespace SimpleOrderingSystem.Tests.Common;

/// <summary>
/// Used to help setup default values for common types inside of TypeMock.
/// </summary>
internal class BehavioralDefaultValueProvider : LookupOrFallbackDefaultValueProvider
{
    public void RegisterDefault<T>(Func<T> valueFunction)
    {
        Register(typeof(T), (type, mock) => valueFunction()!);
    }
}
