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
    public decimal Shipping { get; set;}
    public decimal LineItemTotal {get ;set;}
    public decimal DiscountTotal {get;set;}
    public DateTime CreatedDateTimeUtc { get; init;}
    public DateTime? CancelledDateTimeUtc {get; set;}
    public DateTime? CompletedDateTimeUtc {get; set;}

    public decimal TotalCost => LineItemTotal + Shipping - DiscountTotal;
}
