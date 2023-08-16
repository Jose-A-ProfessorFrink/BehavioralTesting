using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.Domain;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(CreateOrderRequest request);

    Task<Order?> GetOrderAsync(string orderId);

    Task<List<Order>> SearchOrdersAsync(OrderSearchRequest request);
}