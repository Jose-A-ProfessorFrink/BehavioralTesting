using LiteDB;
using SimpleOrderingSystem.Domain.Models;
using SimpleOrderingSystem.Domain.Repositories;
using SimpleOrderingSystem.Repositories.LiteDB.Providers;
using SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;

namespace SimpleOrderingSystem.Repositories;

internal class OrdersRepository: IOrdersRepository
{
    private readonly ILiteDbProvider _liteDbProvider;

    public OrdersRepository(ILiteDbProvider liteDbProvider)
    {
        _liteDbProvider = liteDbProvider;
    }

    public async Task CreateOrderAsync(Order order)
    {
        await _liteDbProvider.CreateOrderAsync(Map(order));

        return;
    }

    public async Task<Order?> GetOrderAsync(Guid id)
    {
        var order = await _liteDbProvider.GetOrderAsync(id);

        return order is null? default: Map(order);
    }

    #region Helpers

    private OrderDataModel Map(Order order)
    {
        return new()
        {

        };
    }

    private Order Map(OrderDataModel order)
    {
        return new()
        {

        };
    }

    #endregion
}
