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

    public async Task<List<Order>> SearchOrdersAsync(DateTime noOlderThanUtc, Guid? customerId)
    {
        var orders = await _liteDbProvider.SearchOrdersAsync(noOlderThanUtc, customerId);

        return orders.Select(order => Map(order)).ToList();
    }

    #region Helpers
    private Order Map(OrderDataModel order)
    {
        return new()
        {
            Id = order.Id,
            Status = order.Status,
            Type = order.Type,
            CreatedDateTimeUtc = order.CreatedDateTimeUtc,
            CancelledDateTimeUtc = order.CancelledDateTimeUtc,
            CompletedDateTimeUtc = order.CompletedDateTimeUtc,
            Shipping = order.Shipping,
            TotalCost = order.TotalCost,
            Customer = Map(order.Customer),
            ShippingAddress = order.ShippingAddress is null? default: Map(order.ShippingAddress),
        };
    }

    private OrderDataModel Map(Order order)
    {
        return new()
        {
            Id = order.Id,
            Status = order.Status,
            Type = order.Type,
            CreatedDateTimeUtc = order.CreatedDateTimeUtc,
            CancelledDateTimeUtc = order.CancelledDateTimeUtc,
            CompletedDateTimeUtc = order.CompletedDateTimeUtc,
            Shipping = order.Shipping,
            TotalCost = order.TotalCost,
            Customer = Map(order.Customer),
            ShippingAddress = order.ShippingAddress is null? default: Map(order.ShippingAddress),
        };
    }

    private CustomerDataModel Map(Customer customer)
    {
        return new()
        {
            Id = customer.Id,
            Name = customer.Name,
            DateHired = customer.DateHired,
            DateOfBirth = customer.DateOfBirth,
            AnnualSalary = customer.AnnualSalary
        };
    }

    private Customer Map(CustomerDataModel customer)
    {
        return new()
        {
            Id = customer.Id,
            Name = customer.Name,
            DateHired = customer.DateHired,
            DateOfBirth = customer.DateOfBirth,
            AnnualSalary = customer.AnnualSalary
        };
    }

    private AddressDataModel Map(Address address)
    {
        return new()
        {
            Line1 = address.Line1,
            Line2 = address.Line2, 
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode
        };
    }

    private Address Map(AddressDataModel address)
    {
        return new()
        {
            Line1 = address.Line1,
            Line2 = address.Line2, 
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode
        };
    }

    #endregion
}
