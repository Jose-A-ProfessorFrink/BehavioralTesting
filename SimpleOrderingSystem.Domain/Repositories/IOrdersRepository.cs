using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.Domain.Repositories;

public interface IOrdersRepository
{
    Task CreateOrderAsync(Order order);

    Task<Order?> GetOrderAsync(Guid id);
}
