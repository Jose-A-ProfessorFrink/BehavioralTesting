using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.Domain.Repositories;

public interface IOrdersRepository
{
    Task CreateOrderAsync(Order order);

    Task UpdateOrderAsync(Order order);

    Task<Order?> GetOrderAsync(Guid id);

    Task<List<Order>> SearchOrdersAsync(DateTime noOlderThanUtc, Guid? customerId);
}
