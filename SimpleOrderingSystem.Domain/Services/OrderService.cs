using SimpleOrderingSystem.Domain.Models;
using SimpleOrderingSystem.Domain.Providers;
using SimpleOrderingSystem.Domain.Repositories;


namespace SimpleOrderingSystem.Domain.Services;

internal class OrderService:IOrderService
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IGuidProvider _guidProvider;
    private readonly IDateTimeProvider _dateTimeProvider;

    public OrderService(IOrdersRepository ordersRepository, IGuidProvider guidProvider, IDateTimeProvider dateTimeProvider)
    {
        _ordersRepository = ordersRepository;
        _guidProvider = guidProvider;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        var newOrder = new Order()
        {
            Id = _guidProvider.NewGuid(),
            
        };

        await Task.CompletedTask;
        
        throw new NotImplementedException();
    }

    public async Task<Order?> GetOrderAsync(string orderId)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}