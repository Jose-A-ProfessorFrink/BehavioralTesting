using SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;
using LiteDB;

namespace SimpleOrderingSystem.Repositories.LiteDB.Providers;

internal interface ILiteDbProvider
{
    Task<CustomerDataModel> GetCustomerAsync(Guid customerId);

    Task<List<CustomerDataModel>> SearchCustomersAsync(string name);

    Task CreateOrderAsync(OrderDataModel order);

    Task<OrderDataModel?> GetOrderAsync(Guid orderId);

    Task<List<OrderDataModel>> SearchOrdersAsync(DateTime noOlderThanUtc, Guid? customerId);
}