namespace SimpleOrderingSystem.Domain.Models;

public class Order
{
    public Guid Id { get; init;}
    public OrderStatus Status { get; set;}
    public OrderType Type { get; set;}
    public Customer Customer { get; set;} = default!;
    public Address? ShippingAddress { get; init;} = default!;
    public List<OrderItem> Items { get; init;} = new List<OrderItem>();
    public List<OrderDiscount> Discounts {get; init;} = new List<OrderDiscount>();
    public decimal Shipping { get; init;}
    public decimal TotalCost { get; init;}
    public DateTime CreatedDateTimeUtc { get; init;}
    public DateTime? CancelledDateTimeUtc {get; init;}
    public DateTime? CompletedDateTimeUtc {get; init;}
}
