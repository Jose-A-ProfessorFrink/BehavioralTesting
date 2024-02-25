using SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;
using SimpleOrderingSystem.Repositories.LiteDB.Providers;

namespace SimpleOrderingSystem.Tests.Common;

internal static class LiteDbProviderMockExtensions
{
    /// <summary>
    /// Setups <see cref="ILiteDbProvider.GetOrderAsync(Guid)"/> to return the passed in valueFunction for any call (all parameter values accepted).
    /// </summary>
    /// <param name="mock"></param>
    /// <param name="valueFunction"></param>
    /// <returns></returns>
    public static Mock<ILiteDbProvider> SetupGetOrderAsync(this Mock<ILiteDbProvider> mock, Func<OrderDataModel?> valueFunction)
    {
        mock
            .Setup(a=> a.GetOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync(valueFunction);

        return mock;
    }
}