using System.Reflection;

namespace SimpleOrderingSystem.Tests.Common;

/// <summary>
/// Used to help setup default values for common types inside of TypeMock.
/// </summary>
internal class BehavioralDefaultValueProvider : DefaultValueProvider
{
    private readonly DefaultValueProvider _originalDefaultValueProvider;
    private readonly Dictionary<Type, Func<object>> registrationDictionary = new Dictionary<Type, Func<object>>();
    private static MethodInfo _getDefaultValueMethodInfo = typeof(DefaultValueProvider).GetMethod(nameof(GetDefaultValue), BindingFlags.Instance | BindingFlags.NonPublic)!;


    public BehavioralDefaultValueProvider(DefaultValueProvider originalDefaultValueProvider)
    {
        _originalDefaultValueProvider = originalDefaultValueProvider;
    }

    /// <summary>
    /// Clears all existing default registrations. This effectively reduces this to default Moq behavior.
    /// </summary>
    public void ClearAllRegistrations() => registrationDictionary.Clear();

    public void RegisterDefault<T>(Func<T> valueFunction, bool allowReplacement = false)
    {
        if (registrationDictionary.ContainsKey(typeof(T)) && allowReplacement is false)
        {
            throw new Exception($"Cannot add default registration for type '{typeof(T).FullName}'. Registration already exists.");
        }

        registrationDictionary.Add(typeof(T), () => valueFunction());
    }

    protected override object GetDefaultValue(Type type, Mock mock)
    {
        if (registrationDictionary.ContainsKey(type))
        {
            return registrationDictionary[type]();
        }

        //Yeah man...it's gross, I know. Got any better ideas? I am all ears...
        return _getDefaultValueMethodInfo.Invoke(_originalDefaultValueProvider, new object[] { type, mock })!;
    }
}
