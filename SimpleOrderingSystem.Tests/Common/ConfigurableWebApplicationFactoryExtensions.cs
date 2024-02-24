using Moq;

namespace SimpleOrderingSystem.Tests.Common;

/// <summary>
/// Extensions for <see cref="IConfigurableWebApplicationFactory"/>.
/// </summary>
public static class ConfigurableWebApplicationFactoryExtensions
{
    /// <summary>
    /// Create a new mock of the given type and adds it to the <see cref="IConfigurableWebApplicationFactory"/>.
    /// </summary>
    /// <typeparam name="T">Type of the mocked object.</typeparam>
    /// <param name="factory"></param>
    /// <returns></returns>
    public static Mock<T> Mock<T>(this IConfigurableWebApplicationFactory factory, MockBehavior behavior = MockBehavior.Default)
        where T : class
    {
        var mock = new Mock<T>(behavior);

        factory.ConfigureTestServices(sc =>
        {
            sc.ReplaceAllWithSingleton(mock.Object);
        });

        return mock;
    }
}
