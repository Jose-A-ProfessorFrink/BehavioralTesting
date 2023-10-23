using FluentAssertions.Equivalency;

namespace SimpleOrderingSystem.Tests.Common;

/// <summary>
/// Moq and FluentAssertions together as one. A better <see cref="It"/>
/// For cases when a failed setup match must fail the test.
/// </summary>
public static class ItShould
{
    /// <summary>
    /// Be. Fails test if equivalence check fails.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    /// <param name="expected"></param>
    /// <returns></returns>
    public static T Be<T>(T expected)
    {
        return It.Is<T>(actual => AssertEquivalence(actual, expected, o => o, string.Empty));
    }

    /// <summary>
    /// Be. Fails test if equivalence check fails.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    /// <param name="expected"></param>
    /// <param name="because"></param>
    /// <returns></returns>
    public static T Be<T>(T expected, string because)
    {
        return It.Is<T>(actual => AssertEquivalence(actual, expected, o => o, because));
    }

    /// <summary>
    /// Be. Fails test if equivalence check fails.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    /// <param name="expected"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static T Be<T>(
        T expected,
        Func<EquivalencyAssertionOptions<T>,
            EquivalencyAssertionOptions<T>> config)
    {
        return It.Is<T>(actual => AssertEquivalence(actual, expected, config, string.Empty));
    }

    /// <summary>
    /// Assert the Equivalence of the Be.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    /// <param name="actual"></param>
    /// <param name="expected"></param>
    /// <param name="config"></param>
    /// <param name="because"></param>
    /// <returns></returns>
    private static bool AssertEquivalence<T>(
        T actual,
        T expected,
        Func<EquivalencyAssertionOptions<T>,
            EquivalencyAssertionOptions<T>> config,
        string because)
    {
        actual.Should().BeEquivalentTo(expected, config, because);

        return true;
    }
}
