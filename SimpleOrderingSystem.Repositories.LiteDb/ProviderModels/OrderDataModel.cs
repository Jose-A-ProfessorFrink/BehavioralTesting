using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;

internal record OrderDataModel
{
    public Guid Id { get; init;}
    public OrderStatus Status { get; init;}
    public OrderType Type { get; init;}
    public CustomerDataModel Customer { get; init;} = default!;
    public AddressDataModel? ShippingAddress { get; init;} = default!;
    public List<OrderItemDataModel> Items { get; init;} = new List<OrderItemDataModel>();
    public decimal Shipping { get; init;}
    public decimal LineItemTotal { get; init;}
    public decimal DiscountTotal {get;init;}
    public List<OrderDiscountDataModel> Discounts {get; init;} = new List<OrderDiscountDataModel>();
    public DateTime CreatedDateTimeUtc { get; init;}
    public DateTime? CancelledDateTimeUtc {get; init;}
    public DateTime? CompletedDateTimeUtc {get; init;}
}