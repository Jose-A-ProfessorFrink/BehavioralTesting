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
    }

    public async Task UpdateOrderAsync(Order order)
    {
        var succeeded = await _liteDbProvider.UpdateOrderAsync(Map(order));

        if(!succeeded)
        {
            throw new Exception("An unexpected error has occurred. Unable to update order by id because order does not exist");
        }
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
            LineItemTotal = order.LineItemTotal,
            DiscountTotal = order.DiscountTotal,
            Customer = Map(order.Customer),
            ShippingAddress = order.ShippingAddress is null? default: Map(order.ShippingAddress),
            Discounts = order.Discounts.Select(a=> Map(a)).ToList(),
            Items = order.Items.Select(a=> Map(a)).ToList()
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
            DiscountTotal = order.DiscountTotal,
            LineItemTotal = order.LineItemTotal,
            Customer = Map(order.Customer),
            ShippingAddress = order.ShippingAddress is null? default: Map(order.ShippingAddress),
            Discounts = order.Discounts.Select(a=> Map(a)).ToList(),
            Items = order.Items.Select(a=> Map(a)).ToList()
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

    private OrderDiscountDataModel Map(OrderDiscount orderDiscount)
    {
        return new()
        {
            Type = orderDiscount.Type,
            PercentDiscount = orderDiscount.PercentDiscount
        };
    }

    private OrderDiscount Map(OrderDiscountDataModel orderDiscount)
    {
        return new()
        {
            Type = orderDiscount.Type,
            PercentDiscount = orderDiscount.PercentDiscount
        };
    }

    private OrderItemDataModel Map(OrderItem orderItem)
    {
        return new()
        {
            MovieId = orderItem.MovieId,
            MovieYear = orderItem.MovieYear,
            MovieMetascore = orderItem.MovieMetascore,
            Price = orderItem.Price,
            Quantity = orderItem.Quantity
        };
    }

    private OrderItem Map(OrderItemDataModel orderItem)
    {
        return new()
        {
            MovieId = orderItem.MovieId,
            MovieYear = orderItem.MovieYear,
            MovieMetascore = orderItem.MovieMetascore,
            Price = orderItem.Price,
            Quantity = orderItem.Quantity
        };
    }

    #endregion
}
