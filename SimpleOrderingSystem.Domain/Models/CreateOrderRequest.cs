
namespace SimpleOrderingSystem.Domain.Models;

public class CreateOrderRequest
{
    public string CustomerId { get; init;} = default!;

    public OrderType Type { get; init; }

    public Address? ShippingAddress {get; init;}
}