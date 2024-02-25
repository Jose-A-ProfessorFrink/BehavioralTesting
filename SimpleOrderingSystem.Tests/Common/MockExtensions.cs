namespace SimpleOrderingSystem.Tests.Common;

/// <summary>
/// Common mock extensions
/// </summary>
public static class MockExtensions
{
    /// <summary>
    /// Defines the default return value for all mocked methods or properties with return type <typeparamref name= "TReturn" /> and 
    /// return type <see cref="Task<typeparamref name="TReturn"/>"/>
    /// NOTE: Invoking this method will automatically replace the <see cref="Mock.DefaultValueProvider"/> with a <see cref="BehavioralDefaultValueProvider"/>
    /// on the specified mock if the mock does not have a <see cref="BehavioralDefaultValueProvider"/> set already.
    /// </summary>
    /// <typeparam name="TReturn">The return type for which to define a default value.</typeparam>
    /// <param name="mock">The mock object.</param>
    /// <param name="valueFunction">A value function that will be invoked to materialize the value when a matching method is invoked.</param>
    /// <remarks>
    /// Default return value is respected only when there is no matching setup for a method call.
    /// </remarks>
    public static Mock<T> WithDefault<T, TReturn>(
        this Mock<T> mock,
        Func<TReturn> valueFunction)
        where T : class
    {
        var provider = mock.DefaultValueProvider as BehavioralDefaultValueProvider;

        if (provider is null)
        {
            provider = new BehavioralDefaultValueProvider();
            mock.DefaultValueProvider = provider;
        }

        provider.RegisterDefault(valueFunction);

        provider.RegisterDefault(() => Task.FromResult(valueFunction()));

        return mock;
    }
}