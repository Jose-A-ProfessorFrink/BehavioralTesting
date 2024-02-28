using System.Linq.Expressions;

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
    /// <param name="registerAsyncVersion">Auto registers a default async version (return type of <see cref="Task{TResult}"/>. True by default.</param>
    /// <remarks>
    /// Default return value is respected only when there is no matching setup for a method call.
    /// </remarks>
    public static Mock<T> WithDefault<T, TReturn>(
        this Mock<T> mock,
        Func<TReturn> valueFunction,
        bool registerAsyncVersion = true)
        where T : class
    {
        var provider = mock.DefaultValueProvider as BehavioralDefaultValueProvider;

        if (provider is null)
        {
            provider = new BehavioralDefaultValueProvider(mock.DefaultValueProvider);
            mock.DefaultValueProvider = provider;
        }

        provider.RegisterDefault(valueFunction);

        if (registerAsyncVersion)
        {
            provider.RegisterDefault(() => Task.FromResult(valueFunction()));
        }

        return mock;
    }

    /// <summary>
    /// Does a <see cref="Moq.MockExtensions.Reset(Mock)"/> and additionally clears all registrations on the default value 
    /// provider on the mock if the mock is using <see cref="BehavioralDefaultValueProvider"/> 
    /// (Invokes the <see cref="BehavioralDefaultValueProvider.ClearAllRegistrations"/> method).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mock"></param>
    public static void ResetBehavioral(this Mock mock)
    {
        mock.Reset();

        // the default value provider is NOT reset when the mock is reset so do this now if we need to
        var behavioralProvider = mock.DefaultValueProvider as BehavioralDefaultValueProvider;
        if (behavioralProvider is not null)
        {
            behavioralProvider.ClearAllRegistrations();
        }
    }
}